using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Roles 
{
    None,
    Seeker,
    Hider
}

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    [Header("Default Server Settings")]
    public string ipAddress = "Any";
    public int port = 26950;

    [Header("Game State")]
    public bool isLobbySession;
    public bool isGameSession;

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
    }

    private void Start()
    {
        //Limit the application to reduce memory usage
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start();
    }

    private void OnApplicationQuit()
    {
        Server.CloseSocket();
    }

    public void StartGame()
    {
        PlayerManager.instance.totalSeeker = 1;
        PlayerManager.instance.SetSeeker();
        
        int _mostVotedMap = MapManager.instance.GetMostVotedMap();
        string _selectedMap = MapManager.instance.mapList[_mostVotedMap].name;
        SceneManager.LoadScene(_selectedMap);
    }

    public void SetWinner(string _winner)
    {
        GameSceneManager.instance.timerStarted = false;
        ServerSend.EndGame(_winner);
        Debug.Log($"{_winner} is the winner");
        EndGame(6.3f);
    }

    public void EndGame(float _timer)
    {
        StartCoroutine(EndGameAfterTimeout(_timer));
    }

    private IEnumerator EndGameAfterTimeout(float _timeout)
    {
        yield return new WaitForSeconds(_timeout);
    
        PlayerManager.instance.ResetPlayersState();

        Debug.Log("Returning back to lobby...");
        SceneManager.LoadScene("Lobby");
    }
}
