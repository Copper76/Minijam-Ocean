using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    private Dictionary<int, int> fuseRecipe = new Dictionary<int, int>();

    private void Awake()
    {
        fuseRecipe[3] = 4;
        fuseRecipe[5] = 6;
        fuseRecipe[15] = 19;
        fuseRecipe[16] = 20;
        fuseRecipe[17] = 21;
        fuseRecipe[18] = 22;
    }

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
            int id = grid[gridID].GetNextSpawn();
            int index;
            switch (id)
            {
                case 7:
                    index = FindClosestSlot(gridID, new int[] {1, gridWidth, gridWidth+1 });
                    if (index != -1)
                    {
                        SetItemID(index, id);
                        SetItemID(index+1, id + 1);
                        SetItemID(index+gridWidth, id + 2);
                        SetItemID(index+ gridWidth+1, id + 3);
                        grid[gridID].SuccessfulTap();
                    }
                    break;
                default:
                    index = FindClosestSlot(gridID);
                    if (index != -1)
                    {
                        SetItemID(index, id);
                        TryFuse(index, id);
                        grid[gridID].SuccessfulTap();
                    }
                    break;
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
                    bool valid = true;

                    if (grid[id].cellItemID == 0)//Found empty space
                    {
                        return id;
                    }
                    if (id % gridWidth != 0)
                    {
                        backUpExplores.Add(id - 1);
                    }
                    if ((id + 1) % gridWidth != 0)
                    {
                        backUpExplores.Add(id + 1);
                    }
                    backUpExplores.Add(id - gridWidth);
                    backUpExplores.Add(id + gridWidth);
                    explored.Add(id);
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

    private int FindClosestSlot(int id, int[] additionalChecks)
    {
        List<int> nextExplores = new List<int>();
        List<int> backUpExplores = new List<int>();
        HashSet<int> explored = new HashSet<int>();
        nextExplores.Add(id);

        return BFS(nextExplores, backUpExplores, explored, additionalChecks);
    }

    private int BFS(List<int> nextExplores, List<int> backUpExplores, HashSet<int> explored, int[] additionalChecks)
    {
        if (nextExplores.Count > 0)
        {
            nextExplores = nextExplores.OrderBy(x => UnityEngine.Random.value).ToList();//Ensure the spawn is more random
            foreach (int id in nextExplores)
            {
                if (!explored.Contains(id) && id < gridWidth * gridHeight && id >= 0)
                {
                    bool valid = true;

                    if (grid[id].cellItemID != 0)//Found empty space
                    {
                        valid = false;
                    }
                    else
                    {
                        foreach (int i in additionalChecks)
                        {
                            int offset = id + i;
                            if (offset >= gridWidth * gridHeight || id < 0 || grid[offset].cellItemID != 0 || (i == 1 && (id+1) % gridWidth == 0))
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (valid)
                    {
                        return id;
                    }
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
            nextExplores.Clear();
            return BFS(backUpExplores, nextExplores, explored, additionalChecks);
        }
        else//nothing to explore so the board is full
        {
            return -1;
        }
    }

    public void TryFuse(int id, int itemID)
    {
        if (fuseRecipe.ContainsKey(itemID))
        {
            List<int> nextExplores = new List<int>();
            HashSet<int> materialFound = new HashSet<int>();
            HashSet<int> explored = new HashSet<int>();
            nextExplores.Add(id);

            if (BFS(nextExplores, ref materialFound, explored, itemID))
            {
                foreach (int i in materialFound)
                {
                    SetItemID(i, 0);
                }
                SetItemID(id, fuseRecipe[itemID]);
            }
        }
    }

    public bool TryBlow(int id, int itemID)
    {
        switch(itemID)
        {
            case 7:
                //Set the left half of the shell to be a smaller shell
                SetItemID(id, 11);
                SetItemID(id + gridWidth, 12);
                SetItemID(id + 1, 0);
                SetItemID(id + gridWidth + 1, 0);
                int index = FindClosestSlot(id+1, new int[] { 1 });
                if (index == -1)//First attempt failed
                {
                    SetItemID(id, 0);
                    SetItemID(id + gridWidth, 0);
                    SetItemID(id + 1, 11);
                    SetItemID(id + gridWidth + 1, 12);
                    index = FindClosestSlot(id, new int[] { 1 });
                    if (index == -1)//Second attempt failed
                    {
                        SetItemID(id, 7);
                        SetItemID(id + 1, 8);
                        SetItemID(id + gridWidth, 9);
                        SetItemID(id + gridWidth + 1, 10);
                        return false;
                    }
                    else
                    {
                        SetItemID(index, 13);
                        SetItemID(index + 1, 14);
                    }
                }
                else
                {
                    SetItemID(index, 13);
                    SetItemID(index+1, 14);
                }
                break;
            case 11:
                SetItemID(id, 15);
                SetItemID(id + gridWidth, 16);
                break;
            case 13:
                SetItemID(id, 17);
                SetItemID(id + 1, 18);
                break;
            default:
                Debug.Log("Error");
                return false;
                break;
        }
        return true;

    }


    private bool BFS(List<int> nextExplores, ref HashSet<int> materialFound, HashSet<int> explored, int itemID)
    {
        if (materialFound.Count == 3)
        {
            return true;
        }
        if (nextExplores.Count > 0)
        {
            while (nextExplores.Count > 0)
            {
                int id = nextExplores.First();
                nextExplores.RemoveAt(0);
                if (!materialFound.Contains(id) && id < gridWidth * gridHeight && id >= 0 && grid[id].cellItemID == itemID && !explored.Contains(id))
                {
                    explored.Add(id);
                    if (id % gridWidth != 0)
                    {
                        nextExplores.Add(id - 1);
                    }
                    if ((id + 1) % gridWidth != 0)
                    {
                        nextExplores.Add(id + 1);
                    }
                    nextExplores.Add(id - gridWidth);
                    nextExplores.Add(id + gridWidth);
                    materialFound.Add(id);
                    if (BFS(nextExplores, ref materialFound,explored, itemID))
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
