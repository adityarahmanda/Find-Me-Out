using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class Server
{
    public static IPAddress IpAddress { get; private set; }
    public static int Port { get; private set; }

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    public static void Start()
    {
        Debug.Log("Starting server...");
        InitializeServerData();

        tcpListener = new TcpListener(IpAddress, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on IP {IpAddress} and port {Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null)
                {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    public static void CloseSocket()
    {
        tcpListener.Stop();
        udpListener.Close();
    }

    private static void InitializeServerData()
    {
        Debug.Log("Initializing server data...");

        string fileName = "serverdata.txt";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if(File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);
            string[] result = text.Split('\n');

            IpAddress = ParseIPAddress(result[0].Trim());;
            Port = Int32.Parse(result[1].Trim());
        } 
        else
        {
            Debug.Log($"{fileName} doesn't exist in Streaming Assets, using default server settings");
            
            IpAddress = ParseIPAddress(NetworkManager.instance.ipAddress);
            Port = NetworkManager.instance.port;
        }
        
        for (int i = 1; i <= PlayerManager.instance.maxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ClientPackets.TCPTestReceived, ServerHandle.TCPTestReceived },
            { (int)ClientPackets.login, ServerHandle.Login },
            { (int)ClientPackets.signUp, ServerHandle.SignUp },
            { (int)ClientPackets.signOut, ServerHandle.SignOut },
            { (int)ClientPackets.enterLobbyRequest, ServerHandle.EnterLobbyRequest },
            { (int)ClientPackets.spawnPlayerRequest, ServerHandle.SpawnPlayerRequest },
            { (int)ClientPackets.voteMap, ServerHandle.VoteMap },
            { (int)ClientPackets.playerIsReady, ServerHandle.PlayerIsReady },
            { (int)ClientPackets.playerInput, ServerHandle.PlayerInput },
            { (int)ClientPackets.setSkin, ServerHandle.SetSkin },
            { (int)ClientPackets.refeshData, ServerHandle.HandleRefresh },
        };
    }

    private static IPAddress ParseIPAddress(string _ipAddress)
    {
        if(_ipAddress == "Any")
        {
            return IPAddress.Any;
        } 
        else
        {
            return IPAddress.Parse(_ipAddress);
        }
    }
}