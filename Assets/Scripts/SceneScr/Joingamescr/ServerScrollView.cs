using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ServerScrollView : MonoBehaviour
{
    public GameObject ButtonTemplate;

    NetworkManager m_NetworkManager;
    MyNetworkDiscovery m_NetworkDiscovery;
    MyNetworkDiscoveryHud m_MyNetworkDiscoveryHud;
    Dictionary<IPAddress, MyDiscoveryResponseData> discoveredServers;

    void Start()
    {
        m_NetworkManager = FindObjectOfType<NetworkManager>();
        m_MyNetworkDiscoveryHud = m_NetworkManager.GetComponent<MyNetworkDiscoveryHud>();
        m_NetworkDiscovery = m_NetworkManager.GetComponent<MyNetworkDiscovery>();
        discoveredServers = m_MyNetworkDiscoveryHud.discoveredServers;
        StartCoroutine(GenButton());
    }

    public IEnumerator GenButton()
    {
        // Destroy the button before gen new one
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if(i != 0) // Does not destroy the Button Template
            {
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }

        yield return new WaitForSeconds(0.3f);

        foreach (var discoveredServer in discoveredServers)
        {
            GameObject genButton = Instantiate(ButtonTemplate);
            genButton.SetActive(true);
            genButton.transform.SetParent(ButtonTemplate.transform.parent);

            JoinButton JB = genButton.GetComponent<JoinButton>();
            JB.SetDataConnection(discoveredServer);
        }
    }

    public void ButtonClicked(KeyValuePair<IPAddress, MyDiscoveryResponseData> discoveredServer)
    {
        if (discoveredServer.Value.PlayerCount < 5)
        {
            string IP = discoveredServer.Key.ToString();
            ushort port = discoveredServer.Value.Port;

            UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
            transport.SetConnectionData(IP, port);
            m_NetworkManager.StartClient();
        }
        else
        {
            Debug.Log("The server is full!");
        }
    }
}