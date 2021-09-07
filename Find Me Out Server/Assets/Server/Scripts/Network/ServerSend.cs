using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

        private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    private static void SendTCPDataToAllPlayers(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            if(Server.clients[i].player != null) 
                Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAllPlayers(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                if(Server.clients[i].player != null) 
                    Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    private static void SendUDPDataToAllPlayers(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            if(Server.clients[i].player != null)
                Server.clients[i].udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAllPlayers(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                if(Server.clients[i].player != null)
                    Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    public static void SendTCPTest(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.TCPTestServer))
        {
            _packet.Write(_toClient);
            _packet.Write(_msg);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SendUDPTest(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.UDPTestServer))
        {
            _packet.Write(_msg);

            SendUDPData(_toClient, _packet);
        }
    }

    public static void LoginFeedback(int _id, bool _value, string _message)
    {
        using (Packet _packet = new Packet((int)ServerPackets.loginFeedback))
        {
            _packet.Write(_value);
            _packet.Write(_message);

            if(_value)
            {
                _packet.Write<User>(Server.clients[_id].user);
            }

            SendTCPData(_id, _packet);
        }
    }

    public static void SignUpFeedback(int _id, bool _value, string _message)
    {
        using (Packet _packet = new Packet((int)ServerPackets.signUpFeedback))
        {
            _packet.Write(_value);
            _packet.Write(_message);

            SendTCPData(_id, _packet);
        }
    }

    public static void SignOutFeedback(int _id, bool _value)
    {
        using (Packet _packet = new Packet((int)ServerPackets.signOutFeedback))
        {
            _packet.Write(_value);

            SendTCPData(_id, _packet);
        }
    }

    public static void EnterLobbyFeedback(int _id, bool _value, string _message = null)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enterLobbyFeedback))
        {
            _packet.Write(_value);
            if(!_value) _packet.Write(_message);

            SendTCPData(_id, _packet);
        }
    }

    public static void SpawnAllPlayers(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnAllPlayers))
        {
            _packet.Write(PlayerManager.instance.GetTotalPlayers());

            foreach(Client _client in Server.clients.Values)
            {
                if(_client.player != null)
                {
                    _packet.Write(_client.player.id);
                    _packet.Write(_client.player.username);
                    _packet.Write(_client.player.transform.position);
                    _packet.Write(_client.player.isReady);
                    _packet.Write(_client.player.MaterialId);
                }
            }

            SendTCPData(_id, _packet);
        }
    }

    public static void SpawnNewPlayer(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnNewPlayer))
        {
            Player _player = Server.clients[_id].player;
            
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.isReady);
            _packet.Write(_player.MaterialId);

            //Send to all players except 
            SendTCPDataToAllPlayers(_id, _packet);
        }
    }

    public static void SendPlayerIsReady(int _id, bool _value)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerIsReady))
        {
            _packet.Write(_id);
            _packet.Write(_value);

            SendTCPDataToAllPlayers(_packet);
        }
    }

    public static void StartGame(int _seekerId, int _mostVotedMap)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startGame))
        {
            _packet.Write(_seekerId);
            _packet.Write(_mostVotedMap);

            SendTCPDataToAllPlayers(_packet);
        }
    }

    public static void SendPlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAllPlayers(_packet);
        }
    }

    public static void SendPlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAllPlayers(_packet);
        }
    }

    public static void SendPlayerAnimation(int _id, string _animation, bool _value)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerAnimation))
        {
            _packet.Write(_id);
            _packet.Write(_animation);
            _packet.Write(_value);

            SendUDPDataToAllPlayers(_packet);
        }
    }

    public static void SendPlayerSkin(int _id, int _materialId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerSkin))
        {
            _packet.Write(_id);           
            _packet.Write(_materialId);

            SendTCPDataToAllPlayers(_packet);
        }
    }

    public static void SendPlayerIsCaught(int _id, bool _value)
    {
        using (Packet _packet = new Packet ((int)ServerPackets.playerIsCaught))
        {
            _packet.Write(_id);
            _packet.Write(_value);

            SendUDPDataToAllPlayers(_packet);
        }
    }

    public static void SendTimerData(int _time)
    {
        using (Packet _packet = new Packet((int)ServerPackets.timerData))
        {
            _packet.Write(_time);

            SendUDPDataToAllPlayers(_packet);
        }
    }

    public static void SendItemData(Vector3 _pos)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemData))
        {
            _packet.Write(_pos);
            SendUDPDataToAllPlayers(_packet);
        }
    }

    public static void SendFootprintData(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.footprintData))
        {
            _packet.Write(_playerId);
            SendUDPDataToAllPlayers(_packet);
        }
    }

    public static void PlayerDisconnected(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_id);

            SendTCPDataToAllPlayers(_packet);
        }
    }

    public static void EndGame(string _winner)
    {
        using (Packet _packet = new Packet((int)ServerPackets.endGame))
        {
            _packet.Write(_winner);

            SendTCPDataToAllPlayers(_packet);
        }
    }
    #endregion
}
