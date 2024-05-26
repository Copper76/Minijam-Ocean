using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GridInfo : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public Vector2 gridOffset;
    public Vector2 cellSize = new Vector2(50.0f, 50.0f);

    public CellInfo[] grid;

    [SerializeField] private Sprite[] itemSprites;

    private void UpdateSprite(CellInfo cellInfo)
    {
        cellInfo.SetSprite(itemSprites[cellInfo.cellItemID]);
    }

    public void SetItemID(ref CellInfo cellInfo, int itemID)
    {
        cellInfo.cellItemID = itemID;
        UpdateSprite(cellInfo);
    }

    public void SetItemID(int gridID, int itemID)
    {
        SetItemID(ref grid[gridID], itemID);
    }

    public int GetItemID(CellInfo cellInfo)
    {
        return cellInfo.cellItemID;
    }

    public int GetItemID(int gridID)
    {
        return GetItemID(grid[gridID]);
    }

    public void Tap(int gridID)
    {
        if (grid[gridID].Tap())
        {
            int index = FindClosestSlot(gridID);
            if (index != -1)
            {
                SetItemID(index, grid[gridID].GetNextSpawn());
                grid[gridID].SuccessfulTap();
            }
        }
    }

    private int FindClosestSlot(int id)
    {
        List<int> nextExplores = new List<int>();
        List<int> backUpExplores = new List<int>();
        HashSet<int> explored = new HashSet<int> ();
        nextExplores.Add(id);

        return BFS(nextExplores, backUpExplores, explored);
    }

    private int BFS(List<int> nextExplores, List<int> backUpExplores, HashSet<int> explored)
    {
        if (nextExplores.Count > 0)
        {
            nextExplores = nextExplores.OrderBy(x => UnityEngine.Random.value).ToList();//Ensure the spawn is more random
            foreach (int id in nextExplores)
            {
                if (!explored.Contains(id) && id < gridWidth * gridHeight && id >= 0)
                {
                    if (grid[id].cellItemID == 0)//Found empty space
                    {
                        return id;
                    }
                    else
                    {
                        if (id % gridWidth != 0)
                        {
                            backUpExplores.Add(id - 1);
                        }
                        if ((id+1) % gridWidth != 0)
                        {
                            backUpExplores.Add(id + 1);
                        }
                        backUpExplores.Add(id-gridWidth);
                        backUpExplores.Add(id+gridWidth);
                        explored.Add(id);
                    }
                }
            }
            nextExplores.Clear();
            return BFS(backUpExplores, nextExplores, explored);
        }
        else//nothing to explore so the board is full
        {
            return -1;
        }
    }

    public void TryFuseBomb(int id, int itemID)
    {
        List<int> nextExplores = new List<int>();
        HashSet<int> materialFound = new HashSet<int>();
        nextExplores.Add(id);

        if (DFS(nextExplores, ref materialFound, itemID))
        {
            foreach (int i in materialFound)
            {
                SetItemID(i, 0);
            }
            SetItemID(id, itemID+1);
        }
    }

    private bool DFS(List<int> nextExplores, ref HashSet<int> materialFound, int itemID)
    {
        if (materialFound.Count == 3)
        {
            return true;
        }
        if (nextExplores.Count > 0)
        {
            foreach (int id in nextExplores)
            {
                if (!materialFound.Contains(id) && id < gridWidth * gridHeight && id >= 0 && grid[id].cellItemID == itemID)
                {
                    List<int> nextLevelExplores = new List<int>();
                    if (id % gridWidth != 0)
                    {
                        nextLevelExplores.Add(id - 1);
                    }
                    if ((id + 1) % gridWidth != 0)
                    {
                        nextLevelExplores.Add(id + 1);
                    }
                    nextLevelExplores.Add(id - gridWidth);
                    nextLevelExplores.Add(id + gridWidth);
                    materialFound.Add(id);
                    if (DFS(nextLevelExplores, ref materialFound, itemID))
                    {
                        return true;
                    }
                    materialFound.Remove(id);
                }
            }
            return false;
        }
        return false;
    }

    public void Shuffle()
    {
        for (int i=grid.Length -1; i>0; i--)
        {
            if (grid[i].cellItemID > 2 || grid[i].cellItemID == 0)
            {
                int j;
                do
                {
                    j = UnityEngine.Random.Range(0, i + 1);
                } while (grid[j].cellItemID == 1 || grid[j].cellItemID == 2);
                CellInfo cell = grid[i];
                grid[i] = grid[j];
                grid[j] = cell;
            }
        }
        //grid = grid.OrderBy(x => UnityEngine.Random.value).ToArray();
        for (int j = 0; j < gridHeight; j++)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                GameObject cell = grid[j * gridHeight + i].gameObject;
                cell.name = (j* gridHeight +i).ToString();
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f * gridWidth / 2 + i * 50f + gridOffset.x, 50f * gridHeight / 2 - j * 50f + gridOffset.y, 0.0f);
            }
        }
    }
}
