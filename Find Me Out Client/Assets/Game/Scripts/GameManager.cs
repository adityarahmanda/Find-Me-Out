using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Roles 
{
    None,
    Seeker,
    Hider
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
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
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void StartGame(int _seekerId, int _mapId)
    {
        PlayerManager.instance.seekerId = _seekerId;
        LoadingScript.instance.sceneName = MapManager.instance.mapList[_mapId].name;
        LoadingScript.instance.LoadingStart();
    }

    public void EndGame(string _winner)
    {
        if(Client.instance.myId == PlayerManager.instance.seekerId)
        {
            PlayerManager.instance.VisibleAllPlayers();
        }

        StartCoroutine(EndGameAfterTimeout(5f, _winner));
    }

    private IEnumerator EndGameAfterTimeout(float _timeout, string _winner)
    {
        yield return new WaitForSeconds(1f);

        GameSceneManager.instance.ShowWinner(_winner);

        yield return new WaitForSeconds(_timeout);

        GameSceneManager.instance.HideWinner();

        yield return new WaitForSeconds(0.4f);

        PlayerManager.instance.HideAllHiders(false);
        PlayerManager.instance.ResetPlayersState();
        LoadingScript.instance.LoadingStart();
    }
}
