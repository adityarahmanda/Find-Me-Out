using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void TCPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.TCPTestReceived))
        {
            _packet.Write(Client.instance.myId);
            
            SendTCPData(_packet);
        }
    }

    public static void Login(string _email, string _password)
    {
        using (Packet _packet = new Packet((int)ClientPackets.login))
        {
            _packet.Write(_email);
            _packet.Write(_password);

            SendTCPData(_packet);
        }
    }

    public static void SignUp(string _username, string _email, string _password, string _confirmationPassword)
    {
        using (Packet _packet = new Packet((int)ClientPackets.signUp))
        {
            _packet.Write(_username);
            _packet.Write(_email);
            _packet.Write(_password);
            _packet.Write(_confirmationPassword);

            SendTCPData(_packet);
        }
    }

    public static void SignOut()
    {
        using (Packet _packet = new Packet((int)ClientPackets.signOut))
        {
            SendTCPData(_packet);
        }
    }

    public static void SendRefreshData()
    {
        bool _data = true;

        using (Packet _packet = new Packet((int)ClientPackets.refeshData))
        {
            _packet.Write(_data);
            SendUDPData(_packet);
        }
    }

    public static void EnterLobbyRequest()
    {
        using (Packet _packet = new Packet((int)ClientPackets.enterLobbyRequest))
        {
            SendTCPData(_packet);
        }
    }

    public static void SpawnPlayerRequest()
    {
        using (Packet _packet = new Packet((int)ClientPackets.spawnPlayerRequest))
        {
            SendTCPData(_packet);
        }
    }

    public static void VoteMap(int _mapId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.voteMap))
        {
            _packet.Write(_mapId);

            SendTCPData(_packet);
        }
    }

    public static void PlayerIsReady(bool _value)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerIsReady))
        {
            _packet.Write(_value);

            SendTCPData(_packet);
        }
    }

    public static void PlayerInput(float _inputX, float _inputZ)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerInput))
        {
            _packet.Write(_inputX);
            _packet.Write(_inputZ);

            SendUDPData(_packet);
        }
    }

    public static void SetSkin(int _materialId)
    {
        using(Packet _packet = new Packet((int)ClientPackets.setSkin))
        {
            _packet.Write(_materialId);
            SendTCPData(_packet);
        }
    }
    #endregion
}
