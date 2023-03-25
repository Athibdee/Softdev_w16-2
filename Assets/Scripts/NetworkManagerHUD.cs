using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[DisallowMultipleComponent]
public class NetworkManagerHUD : MonoBehaviour
{
    NetworkManager m_NetworkManager;

    UnityTransport m_Transport;

    GUIStyle m_LabelTextStyle;

    //private GameObject mainCamera,lobbymanage;

    // This is needed to make the port field more convenient. GUILayout.TextField is very limited and we want to be able to clear the field entirely so we can't cache this as ushort.
    string m_PortString = "7777";
    string m_ConnectAddress = "127.0.0.1";

    public Vector2 DrawOffset = new Vector2(10, 10);

    public Color LabelColor = Color.black;

    public int PrefabSelectIndex; //0 girl , 1 ghost for now

    void Awake()
    {
        // Only cache networking manager but not transport here because transport could change anytime.
        m_NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        m_LabelTextStyle = new GUIStyle(GUIStyle.none);

        //for camera,ui reset , open it if you want to use shutdown and reset camera
        //mainCamera = GameObject.FindWithTag("MainCamera");
        //lobbymanage = GameObject.FindWithTag("LobbyUI");
    }

    void OnGUI()
    {
        m_LabelTextStyle.normal.textColor = LabelColor;

        m_Transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;

        GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(200, 200)));

        if (IsRunning(m_NetworkManager))
        {

            DrawStatusGUI();
        }
        else
        {
            DrawConnectGUI();
        }

        GUILayout.EndArea();
    }

    void DrawConnectGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Address", m_LabelTextStyle);
        GUILayout.Label("Port", m_LabelTextStyle);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        m_ConnectAddress = GUILayout.TextField(m_ConnectAddress);
        m_PortString = GUILayout.TextField(m_PortString);
        if (ushort.TryParse(m_PortString, out ushort port))
        {
            m_Transport.SetConnectionData(m_ConnectAddress, port, "0.0.0.0");
        }
        else
        {
            m_Transport.SetConnectionData(m_ConnectAddress, 7777, "0.0.0.0");
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Host (Server + Client)"))
        {
            //PrefabSelectIndex = 1;

            m_NetworkManager.StartHost(); // have latency !!
            //spawnManager.SpawnPlayerPrefabServerRpc();
            Debug.Log("hello");
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Server"))
        {
            m_NetworkManager.StartServer();
        }

        if (GUILayout.Button("Client"))
        {
            //PrefabSelectIndex = 0;
            m_NetworkManager.StartClient();
            //spawnManager.SpawnPlayerPrefabServerRpc();

        }

        GUILayout.EndHorizontal();
    }

    void DrawStatusGUI()
    {
        if (m_NetworkManager.IsServer)
        {
            var mode = m_NetworkManager.IsHost ? "Host" : "Server";
            GUILayout.Label("Mode: " + mode, m_LabelTextStyle);
            GUILayout.Label($"{mode} active on port: {m_Transport.ConnectionData.Port.ToString()}", m_LabelTextStyle);
            GUILayout.Label("Address: " + m_Transport.ConnectionData.Address, m_LabelTextStyle);
        }
        else
        {
            if (m_NetworkManager.IsConnectedClient)
            {
                GUILayout.Label($"Client connected {m_Transport.ConnectionData.Address}:{m_Transport.ConnectionData.Port.ToString()}", m_LabelTextStyle);
            }
        }

        //if (GUILayout.Button("Shutdown"))  //MOVE TO Lobby Manager.cs
        //{
        //    m_NetworkManager.Shutdown();

        //    //Reset Camera position and view
        //    mainCamera.transform.position = new Vector3(0f, -1.23f, -10f);
        //    mainCamera.GetComponent<Camera>().orthographicSize = 5.05f;

        //    //Reset UI
        //    lobbymanage.GetComponent<LobbyManager>().WaitingGame();
        //}
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsRunning(NetworkManager networkManager) => networkManager.IsServer || networkManager.IsClient; //if network manager start either IsServer or IsClient will be true
}