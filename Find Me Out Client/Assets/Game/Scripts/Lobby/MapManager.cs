using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FindMeOut.Lobby;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public List<Map> mapList = new List<Map>();
    public Transform mapListPanel;
    
    [Header("Prefabs")]
    public MapController mapControllerPrefab;

    [Header("UI Elements")]
    public Button voteMapButton;
    public Button cancelVoteButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        for(int i = 0; i < mapList.Count; i++)
        {
            MapController _mapController = Instantiate(mapControllerPrefab, mapListPanel, false);
            _mapController.Initialize(i, mapList[i]);
        }

        voteMapButton.onClick.AddListener(() => CanvasManager.instance.OpenCanvas(CanvasType.VoteMap, true));
        cancelVoteButton.onClick.AddListener(() => CanvasManager.instance.OpenCanvas(CanvasType.VoteMap, false));
    }
}