using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
//using Unity.VisualScripting.YamlDotNet.Core.Tokens;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

[RequireComponent(typeof(MyNetworkDiscovery))]
public class MyNetworkDiscoveryHud : MonoBehaviour
{
    [SerializeField, HideInInspector]
    MyNetworkDiscovery m_Discovery;

    NetworkManager m_NetworkManager;

    public Dictionary<IPAddress, MyDiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, MyDiscoveryResponseData>();

    public Vector2 DrawOffset = new Vector2(10, 210);
    public Vector2 DrawOffset2 = new Vector2(350, 600);

    void Awake()
    {
        m_Discovery = GetComponent<MyNetworkDiscovery>();
        m_NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (m_Discovery == null) // This will only happen once because m_Discovery is a serialize field
        {
            m_Discovery = GetComponent<MyNetworkDiscovery>();
            UnityEventTools.AddPersistentListener(m_Discovery.OnServerFound, OnServerFound);
            Undo.RecordObjects(new Object[] { this, m_Discovery }, "Set NetworkDiscovery");
        }
    }
#endif

    public void OnServerFound(IPEndPoint sender, MyDiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
    }

    //void OnGUI()
    //{
    //    if (SceneManager.GetActiveScene().name != "Join game")
    //    {
    //        return;
    //    }

    //    GUILayout.BeginArea(new Rect(DrawOffset, DrawOffset2));
    //    if (m_NetworkManager.IsServer || m_NetworkManager.IsClient)
    //    {
    //        if (m_NetworkManager.IsServer)
    //        {
    //            ServerControlsGUI();
    //        }
    //    }
    //    else
    //    {
    //        ClientSearchGUI();
    //    }

    //    GUILayout.EndArea();
    //}

    void ClientSearchGUI()
    {
        if (m_Discovery.IsRunning)
        {
            if (GUILayout.Button("Stop Client Discovery"))
            {
                m_Discovery.StopDiscovery();
                discoveredServers.Clear();
            }

            //if (GUILayout.Button("Refresh List"))
            //{
               // discoveredServers.Clear();
              //  m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
           // }

            GUILayout.Space(40);

            foreach (var discoveredServer in discoveredServers)
            {
                if (GUILayout.Button($"{discoveredServer.Value.ServerName}[{discoveredServer.Key.ToString()}]({discoveredServer.Value.PlayerCount}/5)"))
                {
                    if (discoveredServer.Value.PlayerCount < 5)
                    {
                        UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
                        transport.SetConnectionData(discoveredServer.Key.ToString(), discoveredServer.Value.Port);
                        m_NetworkManager.StartClient();
                    }
                    else
                    {
                        Debug.Log("The server is full!");
                    }
                }
            }
        }
        else
        {
            if (GUILayout.Button("Discover Servers"))
            {
                m_Discovery.StartClient();
                m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
            }
        }
    }

    void ServerControlsGUI()
    {
        if (m_Discovery.IsRunning)
        {
            if (GUILayout.Button("Stop Server Discovery"))
            {
                m_Discovery.StopDiscovery();
            }
        }
        else
        {
            if (GUILayout.Button("Start Server Discovery"))
            {
                m_Discovery.StartServer();
            }
        }
    }
}