using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Game State")]
    public int seekerId;

    [Header("Player Prefabs")]
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    [Header("Other Prefabs")]
    public GameObject fieldOfViewPrefab;
    public GameObject spawnParticle;
    public GameObject caughtParticle;
    public GameObject destroyParticle;
    public GameObject pumkinParticle;
    public GameObject visibleParticle;

    public GameObject footprint;

    [Header("Skin List")]

    private GameObject fieldOfView;
    public Dictionary<int, Player> players;

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

        players = new Dictionary<int, Player>();
    }

    public Player InstantiatePlayer(Vector3 _position, bool _localPlayer = false)
    {
        Player _player;
        if(_localPlayer)
        {
            _player = Instantiate(localPlayerPrefab, _position, Quaternion.identity).GetComponent<Player>();
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, Quaternion.identity).GetComponent<Player>();
        }

        Instantiate(spawnParticle, _position + new Vector3(0, 0.25f, 0), Quaternion.identity);
        return _player;
    }

    public void InstantiateFieldOfView(Transform _player)
    {
        fieldOfView = Instantiate(fieldOfViewPrefab, _player, false);
        fieldOfView.transform.localPosition += new Vector3(0, 0.1f, 0);
    }

    public void AddPlayer(int _id, Player _player)
    {
        _player.transform.parent = this.transform;
        players.Add(_id, _player);
    }

    public void RemovePlayer(int _id)
    {
        Destroy(players[_id].gameObject);
        players.Remove(_id);
    }

    public int GetCaughtPlayers()
    {
        int counter = 0;
        foreach(Player _player in players.Values)
        {
            if(_player.isCaught) counter++;
        }
        return counter;
    }

    public int GetTotalPlayers()
    {
        return players.Count;
    }

    public int GetReadyPlayers()
    {
        int counter = 0;
        foreach(Player _player in players.Values)
        {
            if(_player.isReady) counter++;
        }
        return counter;
    }

    public void VisibleAllPlayers()
    {
        foreach (Player player in players.Values)
        {            
            player.SetInvisible(false);
        }
    }

    public void HideAllHiders(bool _value)
    {
        foreach (Player player in players.Values)
        {
            if (!player.isCaught && player.role == Roles.Hider)
            {
                player.SetInvisible(_value);
            }
        }
    }

    public void FootprintOnPlayer(int _playerId)
    {
        StartCoroutine(Showfootprint(_playerId));
    }

    public IEnumerator Showfootprint(int _playerId)
    {
        players[_playerId].SetFootprint(true);
        yield return new WaitForSeconds(4f);
        players[_playerId].SetFootprint(false);
    }

    public void ResetPlayersState()
    {
        foreach (Player _player in players.Values)
        {            
            _player.isReady = false;
            _player.isCaught = false;
            if(_player.prisonObj != null)
            {
                Destroy(_player.prisonObj);
            }
        }
        Destroy(fieldOfView);
    }
}