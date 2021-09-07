using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    [Header("Spawn Settings")]
    public Transform spawnPos;

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
        NetworkManager.instance.isGameSession = false;
        NetworkManager.instance.isLobbySession = true;

        InitializeLobby();
    }

    public void InitializeLobby()
    {
        //Resetting player position to spawn position
        foreach (Client _client in Server.clients.Values)
        {
            if(_client.player != null)
            {
                _client.player.transform.position = spawnPos.position + new Vector3(Random.Range(-3, 3), 0, Random.Range(-4, 0));
            }
        }
    }

    public void SpawnPlayerInLobby(int _id)
    {
        Player _player = PlayerManager.instance.InstantiatePlayer(spawnPos);
        _player.Initialize(_id);

        Server.clients[_id].player = _player;
        
        //Spawn all players to new client
        ServerSend.SpawnAllPlayers(_id);
        //Spawn client's player to all player
        ServerSend.SpawnNewPlayer(_id);
    }
}
