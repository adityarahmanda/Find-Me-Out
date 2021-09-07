using System.Collections;
using UnityEngine;
using TMPro;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    [Header("UI Elements")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI caughtPlayersText;
    public TextMeshProUGUI timerText;

    [Header("Win Canvas")]
    public TextMeshProUGUI winText;
    public Canvas winCanvas;

    [Header("Object")]
    public GameObject Item;

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
        GameManager.instance.isLobbySession = false;
        GameManager.instance.isGameSession = true;
        
        foreach(Player _player in PlayerManager.instance.players.Values)
        {
            if(PlayerManager.instance.seekerId == _player.id)
            {
                _player.SetRole(Roles.Seeker);
                PlayerManager.instance.InstantiateFieldOfView(_player.transform);
            }
            else
            {
                _player.SetRole(Roles.Hider);
            }
        }

        //If local player is seeker, set invisible to other players
        if(Client.instance.myId == PlayerManager.instance.seekerId)
        {
            statusText.text = "You're a Seeker";
            
            foreach(Player _player in PlayerManager.instance.players.Values)
            {
                if(_player.id != Client.instance.myId)
                {
                    _player.SetInvisible(true);
                }
            }
        }
        else
        {
            statusText.text = "You're a Hider";
        }

        UpdateGameSceneData();
    }

    public void SetTimer(int _time)
    {
        timerText.text = _time.ToString();
    }

    public void UpdateGameSceneData()
    {
        int _caughtPlayers = PlayerManager.instance.GetCaughtPlayers();
        int _totalHiders = PlayerManager.instance.GetTotalPlayers() - 1;

        caughtPlayersText.text = $"{_caughtPlayers}/{_totalHiders}";
    }

    public void ShowWinner(string _winner)
    {
        winText.text = $"{_winner} Wins";
        winCanvas.enabled = true;
        winCanvas.GetComponent<Animator>().SetTrigger("start");
    }

    public void HideWinner()
    {
        winCanvas.GetComponent<Animator>().SetTrigger("end");
    }

    public void SpawnItem(Vector3 _spawnPos)
    {
        Instantiate(Item, _spawnPos, Quaternion.identity);
    }

    public IEnumerator VisibleForSecond(float _second)
    {
        PlayerManager.instance.HideAllHiders(false);
        yield return new WaitForSeconds(_second);
        if(GameManager.instance.isGameSession)
            PlayerManager.instance.HideAllHiders(true);
    }
}
