using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    public const float ONE_SECOND = 1.0f;
    public const int COUNTDOWN_TIME = 60;

    private float timer;
    public int countdownTime;
    public bool timerStarted;

    private const float spawnDuration = 1.5f;
    private float duration;

    [Header("Spawn Positions")]
    public Transform SeekerSpawnPos;
    public Transform HiderSpawnPos;
    public Transform[] ItemSpawnPos;

    [Header("Object")]
    public GameObject Item;

    private List<Transform> SeekerSpawnPosList;
    private List<Transform> HiderSpawnPosList;
    public int itemSpawned;

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
        NetworkManager.instance.isGameSession = true;
        NetworkManager.instance.isLobbySession = false;

        //Instantiate fov as seeker child, world position false
        Player _seeker = PlayerManager.instance.GetSeeker();

        //BUG
        PlayerManager.instance.InstantiateFieldOfView(_seeker.transform);

        InitializeGameScene();

        countdownTime = COUNTDOWN_TIME;
        timer = ONE_SECOND;

        timerStarted = true;
        duration = 3;
    }

    private void Update() 
    {
        Timer();
        SpawnItem();
    }

    private void InitializeGameScene()
    {
        SeekerSpawnPosList = GetChildTransforms(SeekerSpawnPos);
        HiderSpawnPosList = GetChildTransforms(HiderSpawnPos);

        //Resetting player position to spawn position
        foreach (Client _client in Server.clients.Values)
        {
            if(_client.player != null)
            {
                if(_client.player.Role == Roles.Hider)
                {
                    int _randomPos = Random.Range(0, HiderSpawnPosList.Count - 1);
                    _client.player.transform.position = HiderSpawnPosList[_randomPos].position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-1, 1));
                }
                else
                {
                    int _randomPos = Random.Range(0, SeekerSpawnPosList.Count - 1);
                    _client.player.transform.position = SeekerSpawnPosList[_randomPos].position;
                }
            }
        }

        int _seekerId = PlayerManager.instance.seekerId;
        int _mostVotedMap = MapManager.instance.GetMostVotedMap();
        ServerSend.StartGame(_seekerId, _mostVotedMap);
    }

    private List<Transform> GetChildTransforms(Transform _parent)
    {
        List<Transform> _list = new List<Transform>();

        foreach(Transform _child in _parent)
        {
            _list.Add(_child);
        }

        return _list;
    }

    private void Timer()
    {
        if(timerStarted)
        {
            if (countdownTime <= 0) return;

            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                countdownTime--;
                ServerSend.SendTimerData(countdownTime);

                if (countdownTime <= 0)
                {
                    NetworkManager.instance.SetWinner("Hider");
                }

                timer = ONE_SECOND;
            }
        }        
    }
    private void SpawnItem()
    {
        if(timerStarted && itemSpawned <= 5)
        {
            if(duration <= 0)
            {
                int _random = Random.Range(0, ItemSpawnPos.Length);
                Vector3 _spawnPos = ItemSpawnPos[_random].position + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
                ServerSend.SendItemData(_spawnPos);

                Instantiate(Item, _spawnPos, Quaternion.identity);
                itemSpawned++;
                duration = spawnDuration;
            }
            else
            {
                duration -= Time.deltaTime;
            }
        }               
    }
}
