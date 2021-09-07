using UnityEngine;

public class ServerHandle
{
    public static void TCPTestReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();  

        Client _client = Server.clients[_fromClient];

        Debug.Log($"{_client.tcp.socket.Client.RemoteEndPoint} connected successfully with client id {_fromClient}");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player ID: {_fromClient} has assumed the wrong client ID ({_clientIdCheck})!");
        }
    }

    public static void Login(int _fromClient, Packet _packet)
    {
        string _email = _packet.ReadString();
        string _password = _packet.ReadString();

        AuthManager.instance.Login(_fromClient, _email, _password);
    }
    public static void SignUp(int _fromClient, Packet _packet)
    {
        string _username = _packet.ReadString();
        string _email = _packet.ReadString();
        string _password = _packet.ReadString();
        string _passwordConfirmation = _packet.ReadString(); 

        AuthManager.instance.SignUp(_fromClient, _username, _email, _password, _passwordConfirmation);
    }

    public static void SignOut(int _fromClient, Packet _packet)
    {
        AuthManager.instance.SignOut(_fromClient);
    }

    public static void HandleRefresh(int _fromClient, Packet _packet)
    {
        bool _data = _packet.ReadBool();
    }

    public static void EnterLobbyRequest(int _fromClient, Packet _packet)
    {
        if(NetworkManager.instance.isLobbySession)
        {   
            ServerSend.EnterLobbyFeedback(_fromClient, true);
        } 
        else if(NetworkManager.instance.isGameSession)
        {
            ServerSend.EnterLobbyFeedback(_fromClient, false, "Failed entering lobby, game is already started");
        }
    }

    public static void VoteMap(int _fromClient, Packet _packet)
    {
        int _mapId = _packet.ReadInt();
        
        MapManager.instance.VoteMap(_mapId);
    }

    public static void SpawnPlayerRequest(int _fromClient, Packet _packet)
    {
        LobbyManager.instance.SpawnPlayerInLobby(_fromClient);
    }

    public static void PlayerIsReady(int _fromClient, Packet _packet)
    {
        bool _value = _packet.ReadBool();

        Server.clients[_fromClient].player.SetIsReady(_value);

        if(PlayerManager.instance.AllPlayerIsReady())
        {
            Debug.Log("All players is ready, starting game...");
            NetworkManager.instance.StartGame();
        }
    }

    public static void PlayerInput(int _fromClient, Packet _packet)
    {           
        float _inputX = _packet.ReadFloat();
        float _inputZ = _packet.ReadFloat();
        
        Server.clients[_fromClient].player.SetInput(_inputX, _inputZ);

    }

    public static void SetSkin(int _fromClient, Packet _packet)
    {
        int _materialId = _packet.ReadInt();
        Server.clients[_fromClient].player.SetMaterialId(_materialId);
        ServerSend.SendPlayerSkin(_fromClient, _materialId);
    }
}