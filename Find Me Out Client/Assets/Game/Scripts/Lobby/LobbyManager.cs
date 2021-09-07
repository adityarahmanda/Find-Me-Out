using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    [Header("Player Ready Information")]
    public Transform playerReadyInfoPanel;
    public PlayerReadyInfo playerReadyInfoPrefab;
    private Dictionary<int, PlayerReadyInfo> playerReadyInfos;
    
    [Header("UI Elements")]
    public TextMeshProUGUI readyPlayersText;
    public TextMeshProUGUI totalPlayersText;
    public Button readyButton;

    private TextMeshProUGUI readyButtonText;

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
        
        readyButtonText = readyButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.instance.isLobbySession = true;
        GameManager.instance.isGameSession = false;

        if(!PlayerManager.instance.players.ContainsKey(Client.instance.myId))
        {
            ClientSend.SpawnPlayerRequest();
        }
        InitializePlayerReadyInfoUI();
        RefreshLobbyDataUI();

        readyButton.onClick.AddListener(ToggleIsReady);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ToggleIsReady();
        }
    }

    private void InitializePlayerReadyInfoUI()
    {
        playerReadyInfos = new Dictionary<int, PlayerReadyInfo>();
        
        if(PlayerManager.instance.players.Values.Count > 0)
        {
            foreach (Player _player in PlayerManager.instance.players.Values)
            {
                InstantiatePlayerReadyInfo(_player);
            }
        }
    }

    public void SpawnPlayerInLobby(int _id, string _username, Vector3 _position, bool _isReady, int _materialList)
    { 
        Player _player;
        if(Client.instance.myId == _id)
        {
            _player = PlayerManager.instance.InstantiatePlayer(_position, true);
        }
        else
        {
            _player = PlayerManager.instance.InstantiatePlayer(_position);
        }
        _player.Initialize(_id, _username, _isReady, _materialList);

        PlayerManager.instance.AddPlayer(_id, _player);
        InstantiatePlayerReadyInfo(_player);
    }

    private void ToggleIsReady()
    {
        bool _value = !PlayerManager.instance.players[Client.instance.myId].isReady;
        
        ClientSend.PlayerIsReady(_value);
    }

    public void SetReadyButtonText(bool _value)
    {
        if(_value)
            readyButtonText.text = "Cancel";
        else
            readyButtonText.text = "Ready";
    }

    public void InstantiatePlayerReadyInfo(int _playerId)
    {
        Player _player = PlayerManager.instance.players[_playerId];
        InstantiatePlayerReadyInfo(_player);
    }

    public void InstantiatePlayerReadyInfo(Player _player)
    {
        PlayerReadyInfo _playerReadyInfo = Instantiate(playerReadyInfoPrefab, playerReadyInfoPanel, false);
        _playerReadyInfo.SetUsernameText(_player.username);
        _playerReadyInfo.SetIsReadyText(_player.isReady);

        playerReadyInfos.Add(_player.id, _playerReadyInfo);
    }

    public void RemovePlayerReadyInfo(int _playerId)
    {
        Destroy(playerReadyInfos[_playerId].gameObject);
        playerReadyInfos.Remove(_playerId);
    }

    public void SetPlayerReadyInfo(int _playerId, bool _isReady)
    {
        playerReadyInfos[_playerId].SetIsReadyText(_isReady);
    }

    public void RefreshLobbyDataUI()
    {
        readyPlayersText.text = PlayerManager.instance.GetReadyPlayers().ToString();
        totalPlayersText.text = PlayerManager.instance.GetTotalPlayers().ToString();
    }
}
