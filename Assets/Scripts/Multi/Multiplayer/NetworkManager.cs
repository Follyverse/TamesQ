using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }
    public Server Server { get; private set; }

    //   public static string commandIP;
    //   [SerializeField] private string ip;
    //   [SerializeField] private ushort port;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        //  Debug.Log("starete");
        if (CoreTame.IsServer)
        {
            Server = new Server();
            Server.ClientDisconnected += PlayerLeft;
            Server.Start(ushort.Parse(CoreTame.Port), 32);
        }
        else if (CoreTame.multiPlayer)
        {
            Client = new Client();
            Client.Connected += DidConnect;
            Client.ConnectionFailed += FailedToConnect;
            Client.ClientDisconnected += PlayerLeft;
            Client.Disconnected += DidDisconnect;
        }
    }

    private void FixedUpdate()
    {
        if (CoreTame.multiPlayer)
        {
            if (CoreTame.IsServer) Server.Tick();
            else Client.Tick();
        }
    }

    private void OnApplicationQuit()
    {
        if (CoreTame.multiPlayer)
        {
            if (!CoreTame.IsServer)
                Client.Disconnect();
            else
                Server.Stop();
        }
    }

    public void Connect()
    {
        if (CoreTame.Port != "")
            Client.Connect($"{CoreTame.Address}:{CoreTame.Port}");
        else
            Client.Connect($"{CoreTame.Address}");
    }


    private void DidConnect(object sender, EventArgs e)
    {
        Player.SendName();
        Debug.Log("NM: connected");
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        CoreTame.multiPlayer = false;
        CoreTame.loadStatus = CoreTame.LoadStatus.ConnectionChecked;
        Debug.Log("failed to connect. Solo user activated");
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Debug.Log("disconnected: " + e.Id);
        Player.Disconnect(e.Id);
        //        Server.
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        Debug.Log("NM: disconnected");

    }
}
