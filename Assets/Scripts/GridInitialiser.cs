using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridInitialiser : MonoBehaviour
{
    [SerializeField] GridInfo gridInfo;

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    // Start is called before the first frame update
    void Awake()
    {
        int gridSize = gridWidth * gridHeight;
        gridInfo.grid = new CellInfo[gridSize];

        for (int j = 0; j < gridHeight; j++)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                int index = j * gridHeight + i + 1;
                GameObject cell = new GameObject(index.ToString());
                cell.transform.parent = transform;
                cell.layer = 6;
                Image image = cell.AddComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 0.2f);
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f + i * 60f, 100f - j * 60f, 0.0f);
                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                gridInfo.grid[index] = new CellInfo(cell);
            }
        }

        Destroy(this);
    }
}
