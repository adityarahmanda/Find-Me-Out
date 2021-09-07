using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FindMeOut.Lobby;

public class MapController : MonoBehaviour
{ 
    private int mapId;

    public TextMeshProUGUI mapNameText;
    public Image mapThumbnail;
    public Button mapVoteButton;

    public void Initialize(int _mapId, Map _map)
    {
        mapId = _mapId;
        mapNameText.text = _map.name;
        mapThumbnail.sprite = _map.thumbnail;
        
        mapVoteButton.onClick.AddListener(Vote);
    }

    public void Vote()
    {
        ClientSend.VoteMap(mapId);
        CanvasManager.instance.OpenCanvas(CanvasType.VoteMap, false);
    }
}