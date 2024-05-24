using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct CellInfo
{
    public GameObject cell;
    public int cellItemID;

    public CellInfo(GameObject cell, int id = 0)
    {
        this.cell = cell;
        cellItemID = id;
    }
}

public class GridInfo : MonoBehaviour
{
    public CellInfo[] grid;
    [SerializeField] private Sprite[] itemSprites;

    public void UpdateSprite(CellInfo cellInfo)
    {
        cellInfo.cell.GetComponent<Image>().sprite = itemSprites[cellInfo.cellItemID];
    }

    public void UpdateSprite(int gridID)
    {
        UpdateSprite(grid[gridID]);
    }

    public void SetItemID(CellInfo cellInfo, int itemID)
    {
        cellInfo.cellItemID = itemID;
        UpdateSprite(cellInfo);
    }

    public void SetItemID(int gridID, int itemID)
    {
        SetItemID(grid[gridID], itemID);
    }
}
