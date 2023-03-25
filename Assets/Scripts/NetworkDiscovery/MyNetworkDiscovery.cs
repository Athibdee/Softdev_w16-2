using System;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

public class MyNetworkDiscovery : NetworkDiscovery<DiscoveryBroadcastData, MyDiscoveryResponseData>
{
    [Serializable]
    public class ServerFoundEvent : UnityEvent<IPEndPoint, MyDiscoveryResponseData>
    {
    };

    NetworkManager m_NetworkManager;

    [SerializeField]
    [Tooltip("If true NetworkDiscovery will make the server visible and answer to client broadcasts as soon as netcode starts running as server.")]
    bool m_StartWithServer = true;

    public string ServerName = "EnterName";

    public ServerFoundEvent OnServerFound;

    private bool m_HasStartedWithServer = false;

    public void Awake()
    {
        m_NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void Update()
    {
        if (m_StartWithServer && m_HasStartedWithServer == false && IsRunning == false)
        {
            if (m_NetworkManager.IsServer)
            {
                StartServer();
                m_HasStartedWithServer = true;
            }
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out MyDiscoveryResponseData response)
    {
        response = new MyDiscoveryResponseData()
        {
            ServerName = ServerName,
            Port = ((UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
            PlayerCount = m_NetworkManager.ConnectedClientsIds.Count,
        };
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, MyDiscoveryResponseData response)
    {
        OnServerFound.Invoke(sender, response);
    }
}