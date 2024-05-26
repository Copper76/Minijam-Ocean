using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.TextCore;
using System;

public class GridInitialiser : MonoBehaviour
{
    [SerializeField] GridInfo gridInfo;
    [SerializeField] string spawnDataFile;
    [SerializeField] string placementData;
    [SerializeField] TutorialController tutorialController;

    private Dictionary<int, int[]> spawnInfo;
    private Dictionary<int, int> placeInfo;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadJsonFromFile(Path.Combine(Application.streamingAssetsPath, spawnDataFile), OnJsonLoadedSpawnInfo));
        StartCoroutine(LoadJsonFromFile(Path.Combine(Application.streamingAssetsPath, placementData), OnJsonLoadedPlaceInfo));
    }

    IEnumerator LoadJsonFromFile(string url, Action<string> onLoaded)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error");
        }
        else
        {
            onLoaded?.Invoke(request.downloadHandler.text);
        }
    }

    void OnJsonLoadedSpawnInfo(string jsonText)
    {
        spawnInfo = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(jsonText);

        TryInitialiseGame();
    }

    void OnJsonLoadedPlaceInfo(string jsonText)
    {
        placeInfo = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonText);

        TryInitialiseGame();
    }

    private void TryInitialiseGame()
    {
        if (spawnInfo != null && placeInfo != null)
        {
            if (tutorialController != null)
            {
                tutorialController.StartLevel();
            }
            InitialiseGame();
        }
    }

    void InitialiseGame()
    {

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
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f * gridInfo.gridWidth / 2 + i * 50f + gridInfo.gridOffset.x, 50f * gridInfo.gridHeight / 2 - j * 50f + gridInfo.gridOffset.y, 0.0f);
                cell.GetComponent<RectTransform>().sizeDelta = gridInfo.cellSize;
                gridInfo.grid[index] = cellInfo;
                if (placeInfo.ContainsKey(index))
                {
                    int id = placeInfo[index];
                    gridInfo.SetItemID(index, id);
                    switch (id)
                    {
                        case 1:
                            cellInfo.SetLimits(2, 10.0f);
                            cellInfo.SetSpawnList(spawnInfo[id]);
                            break;
                        case 2:
                            cellInfo.SetLimits(2, 10.0f);
                            cellInfo.SetSpawnList(spawnInfo[id]);
                            break;
                    }
                }
            }
        }
        Destroy(this);
    }
}
