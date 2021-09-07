using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void TCPTestHandler(Packet _packet)
    {
        int _myId = _packet.ReadInt(); 
        string _message = _packet.ReadString();

        Client.instance.myId = _myId;
        Debug.Log(_message);       
        
        ClientSend.TCPTestReceived();
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UDPTestHandler(Packet _packet)
    {
        string _message = _packet.ReadString();
        Debug.Log(_message);

        Client.instance.isConnected = true;
        MainMenuManager.instance.ConnectionFeedback();
    }

    public static void LoginFeedback(Packet _packet)
    {
        bool _value = _packet.ReadBool();
        string _message = _packet.ReadString();

        if(_value)
        {
            Client.instance.user = _packet.ReadObject<User>();
        }
        
        MainMenuManager.instance.LoginFeedback(_value, _message);
    }

    public static void SignUpFeedback(Packet _packet)
    {
        bool _value = _packet.ReadBool();
        string _message = _packet.ReadString();
        
        MainMenuManager.instance.SignUpFeedback(_value, _message);
    }

    public static void SignOutFeedback(Packet _packet)
    {
        bool _value = _packet.ReadBool();

        MainMenuManager.instance.SignOutFeedback(_value);
    }

    public static void EnterLobbyFeedback(Packet _packet)
    {
        bool _value = _packet.ReadBool();
        if(!_value)
        {
            string _message = _packet.ReadString();
            MainMenuManager.instance.EnterLobbyFeedback(_value, _message);
        }
        else
        {
            MainMenuManager.instance.EnterLobbyFeedback(_value);
        }
    }

    public static void SpawnAllPlayers(Packet _packet)
    {
        int _playersLength = _packet.ReadInt();

        for(int i = 0; i < _playersLength; i++)
        {
            int _id = _packet.ReadInt();
            string _username = _packet.ReadString();
            Vector3 _position = _packet.ReadVector3();
            bool _isReady = _packet.ReadBool();
            int _textureId = _packet.ReadInt();

            LobbyManager.instance.SpawnPlayerInLobby(_id, _username, _position, _isReady, _textureId);

            if(_id == Client.instance.myId)
            {
                WardrobeManager.instance.SetCharacterTexture(_textureId);
            }
        }
        LobbyManager.instance.RefreshLobbyDataUI();
    }

    public static void SpawnNewPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        bool _isReady = _packet.ReadBool();
        int _textureId = _packet.ReadInt();

        LobbyManager.instance.SpawnPlayerInLobby(_id, _username, _position, _isReady, _textureId);
        LobbyManager.instance.RefreshLobbyDataUI();
    }

    public static void PlayerIsReady(Packet _packet)
    {
        int _id = _packet.ReadInt();
        bool _isReady = _packet.ReadBool();

        if(PlayerManager.instance.players.ContainsKey(_id))
        {
            PlayerManager.instance.players[_id].SetIsReady(_isReady);
            if(_id == Client.instance.myId)
            {
                LobbyManager.instance.SetReadyButtonText(_isReady);
            }
            LobbyManager.instance.SetPlayerReadyInfo(_id, _isReady);
            LobbyManager.instance.RefreshLobbyDataUI();
        }
    }

    public static void StartGame(Packet _packet)
    {
        int _seekerId = _packet.ReadInt();
        int _mapId = _packet.ReadInt();

        GameManager.instance.StartGame(_seekerId, _mapId);
    }

    public static void PlayerPosition(Packet _packet)
    {   
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        if(PlayerManager.instance.players.ContainsKey(_id))
        {
            PlayerManager.instance.players[_id].SetPosition(_position);
        }
    }

    public static void PlayerRotation(Packet _packet)
    {   
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        if(PlayerManager.instance.players.ContainsKey(_id))
        {
            PlayerManager.instance.players[_id].SetRotation(_rotation);
        }
    }

    public static void PlayerAnimation(Packet _packet)
    {   
        int _id = _packet.ReadInt();
        string _animation = _packet.ReadString();
        bool _value = _packet.ReadBool();

        if(PlayerManager.instance.players.ContainsKey(_id))
        {
            PlayerManager.instance.players[_id].SetAnimation(_animation, _value);
        }
    }

    public static void PlayerSkin(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _textureId = _packet.ReadInt();
        if (PlayerManager.instance.players.ContainsKey(_id))
        {
            PlayerManager.instance.players[_id].SetTexture(_textureId);
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {   
        int _id = _packet.ReadInt();

        if(PlayerManager.instance.players.ContainsKey(_id))
        {
            PlayerManager.instance.RemovePlayer(_id);

            if(GameManager.instance.isLobbySession)
            {
                LobbyManager.instance.RemovePlayerReadyInfo(_id);
                LobbyManager.instance.RefreshLobbyDataUI();
            }
            else if(GameManager.instance.isGameSession)
            {
                GameSceneManager.instance.UpdateGameSceneData();
            }
            Debug.Log($"Player {_id} is disconnected");
        }
    }

    public static void PlayerIsCaught(Packet _packet)
    {
        int _id = _packet.ReadInt();
        bool _value = _packet.ReadBool();

        if(PlayerManager.instance.players.ContainsKey(_id))
        {
            PlayerManager.instance.players[_id].SetIsCaught(_value);
            GameSceneManager.instance.UpdateGameSceneData();
        }
    }

    public static void TimerData(Packet _packet)
    {
        int _timer = _packet.ReadInt();
        if(GameManager.instance.isGameSession)
        {
            GameSceneManager.instance.SetTimer(_timer);
        }        
    }

    public static void ItemData(Packet _packet)
    {
        Vector3 _spawnPos = _packet.ReadVector3();
        if (GameManager.instance.isGameSession)
        {
            GameSceneManager.instance.SpawnItem(_spawnPos);
        }
    }

    public static void FootprintData(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        if (GameManager.instance.isGameSession)
        {
            PlayerManager.instance.FootprintOnPlayer(_playerId);  
        }
    }
    
    public static void EndGame(Packet _packet)
    {
        string _winner = _packet.ReadString();

        GameManager.instance.EndGame(_winner);
    }
}
