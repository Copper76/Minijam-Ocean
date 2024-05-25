using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class GridInitialiser : MonoBehaviour
{
    [SerializeField] GridInfo gridInfo;
    [SerializeField] string spawnDataFile;

    // Start is called before the first frame update
    void Start()
    {
        Dictionary<int, int[]> spawnInfo = new Dictionary<int, int[]>();
        string filePath = Path.Combine(Application.dataPath, spawnDataFile);
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            spawnInfo = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(jsonString);
            //spawnInfo = JsonUtility.FromJson<Dictionary<int, int[]>>(jsonString);
            //Debug.Log(string.Join(",",spawnInfo[1]));
        }

        int gridSize = gridInfo.gridWidth * gridInfo.gridHeight;
        gridInfo.grid = new CellInfo[gridSize];

        for (int j = 0; j < gridInfo.gridHeight; j++)
        {
            for (int i = 0; i < gridInfo.gridWidth; i++)
            {
                int index = j * gridInfo.gridHeight + i;
                GameObject cell = new GameObject(index.ToString());
                cell.transform.parent = transform;
                cell.layer = 6;
                CellInfo cellInfo = cell.AddComponent<CellInfo>();
                cellInfo.SetImage(cell.AddComponent<Image>());
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f * gridInfo.gridWidth /2 + i * 60f + gridInfo.gridOffset.x, 50f * gridInfo.gridHeight / 2 - j * 60f + gridInfo.gridOffset.y, 0.0f);
                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                gridInfo.grid[index] = cellInfo;
                int id = Random.Range(0, 3);
                gridInfo.SetItemID(index, id);
                switch (id)
                {
                    case 1:
                    cellInfo.SetLimits(2, 10.0f);
                    cellInfo.SetSpawnList(spawnInfo[id]);
                    break;
                }
            }
        }

        Destroy(this);
    }
}
