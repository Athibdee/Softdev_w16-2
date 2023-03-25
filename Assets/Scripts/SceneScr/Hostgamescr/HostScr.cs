using System;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostScr : MonoBehaviour
{
    NetworkManager m_NetworkManager;
    UnityTransport m_Transport;
    MyNetworkDiscovery m_NetworkDiscovery;
    string ServerName;

    private void Start()
    {
        m_NetworkManager = FindObjectOfType<NetworkManager>();
        m_NetworkDiscovery = m_NetworkManager.GetComponent<MyNetworkDiscovery>();
        m_Transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
    }
    public void Hostgame()
    {
        ServerName = GameObject.FindWithTag("ServerName").GetComponent<TMP_InputField>().text;
        m_NetworkDiscovery.ServerName = ServerName;

        m_Transport.SetConnectionData("127.0.0.1", 7778, "0.0.0.0");
        m_NetworkManager.StartHost();
        m_NetworkDiscovery.StartServer();
        m_NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void Backgame()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
