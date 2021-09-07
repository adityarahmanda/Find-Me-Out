using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public int minPlayers = 2;
    public int maxPlayers = 12;

    [Header("Game State")]
    public int totalSeeker = 1;
    public int seekerId = 0;

    private GameObject fieldOfView;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject fieldOfViewPrefab;
    
    public Dictionary<Transform, Player> caughtPlayers;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        caughtPlayers = new Dictionary<Transform, Player>();
    }

    public Player InstantiatePlayer(Transform _transform)
    {
        Player _player = Instantiate(playerPrefab, _transform.position + new Vector3(Random.Range(-3, 3), 0, Random.Range(-4, 0)), _transform.rotation).GetComponent<Player>();
        _player.transform.parent = this.transform;

        return _player;
    }

    public void InstantiateFieldOfView(Transform _player)
    {
        fieldOfView = Instantiate(fieldOfViewPrefab, _player, false);
    }

    public void SetSeeker()
    {
        seekerId = GetRandomPlayerId();

        foreach (Client _client in Server.clients.Values)
        {
            if(_client.player != null) 
            {
                if (_client.id == seekerId)
                    _client.player.Role = Roles.Seeker;
                else
                    _client.player.Role = Roles.Hider;
            }
        }
    }

    public Player GetSeeker()
    {
        return Server.clients[seekerId].player;
    }

    public bool AllPlayerIsReady()
    {
        int _totalPlayers = GetTotalPlayers();
        int _readyPlayers = GetReadyPlayers();

        if(_totalPlayers >= minPlayers && _totalPlayers == _readyPlayers)
        {
            return true;
        }

        return false;
    }

    public bool IsPlayerCaught(Transform _playerTransform)
    {
        return caughtPlayers.ContainsKey(_playerTransform);
    }

    public void CatchPlayer(Transform _playerTransform)
    {
        Player _player = _playerTransform.GetComponent<Player>();
        CatchPlayer(_player);
    }

    public void CatchPlayer(Player _player)
    {
        _player.SetPlayerIsCaught(true);
        caughtPlayers.Add(_player.transform, _player);
        Debug.Log($"Seeker has caught {_player.username}");
        CheckCaughtPlayer();
    }

    public void ReleasePlayer(Transform _playerTransform)
    {
        Player _player = _playerTransform.GetComponent<Player>();
        ReleasePlayer(_player);
    }

    public void ReleasePlayer(Player _player)
    {
        _player.SetPlayerIsCaught(false);
        caughtPlayers.Remove(_player.transform);
        Debug.Log($"{_player.username} is released");

        int _caughtPlayers = GetCaughtPlayers();
        int _totalHiders = GetTotalPlayers() - 1;
        int _hidersLeft = _totalHiders - _caughtPlayers;
        Debug.Log($"There are {_hidersLeft} hiders left");
    }

    public void CheckCaughtPlayer()
    {
        int _caughtPlayers = GetCaughtPlayers();
        int _totalHiders = GetTotalPlayers() - totalSeeker;

        if (_caughtPlayers == _totalHiders)
        {
            Debug.Log($"All hiders are caught");
            NetworkManager.instance.SetWinner("Seeker");
        }
        else
        {
            int _hidersLeft = _totalHiders - _caughtPlayers;
            Debug.Log($"There are {_hidersLeft} hiders left");
        }
    }

    public int GetRandomPlayerId()
    {
        int _totalPlayers = GetTotalPlayers();
        int _random = Random.Range(0, _totalPlayers);

        int _counter = 0;
        foreach (Client _client in Server.clients.Values)
        {
            if(_client.player != null)
            {
                if(_random == _counter) return _client.id;
                _counter++;
            }
        }
        return 0;
    }

    public int GetReadyPlayers()
    {
        int readyPlayers = 0;
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null && _client.player.isReady) readyPlayers++;
        }
        return readyPlayers;
    }
    
    public int GetTotalPlayers()
    {
        int playerCounts = 0;
        foreach (Client _client in Server.clients.Values)
        {
            if(_client.player != null) playerCounts++;
        }
        return playerCounts;
    }

    public int GetCaughtPlayers()
    {
        return caughtPlayers.Count;
    }

    public void ResetPlayersState()
    {
        seekerId = 0;

        foreach (Client _client in Server.clients.Values)
        {
            if(_client.player != null) 
            {
                _client.player.Role = Roles.None;
                _client.player.isReady = false;
                _client.player.isCaught = false;
                _client.player.speed = 4;
                caughtPlayers.Clear();
            }
        }
        Destroy(fieldOfView);
    }

    public void DisconnectionHandler(int _playerId)
    {
        if(Server.clients[_playerId].player.Role == Roles.Seeker)
        {
            totalSeeker--;
            Debug.Log($"Seeker with client's id {_playerId} is disconnected from total : {totalSeeker}");

            Server.clients[_playerId].DestroyPlayer();
            if (totalSeeker == 0)
            {
                Debug.Log($"No more seekers left");
                NetworkManager.instance.SetWinner("Hider");            
            }
        }
        else if(Server.clients[_playerId].player.Role == Roles.Hider)
        {
            Debug.Log($"Hider with client's id {_playerId} is disconnected");

            Server.clients[_playerId].DestroyPlayer();
            CheckCaughtPlayer();
        }
    }
}
