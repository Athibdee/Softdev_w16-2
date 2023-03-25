using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinScr : MonoBehaviour
{
    NetworkManager m_NetworkManager;
    MyNetworkDiscovery m_NetworkDiscovery;
    MyNetworkDiscoveryHud m_MyNetworkDiscoveryHud;
    public GameObject severScrollView;

    private void Start()
    {
        m_NetworkManager = FindObjectOfType<NetworkManager>();
        m_MyNetworkDiscoveryHud = m_NetworkManager.GetComponent<MyNetworkDiscoveryHud>();
        m_NetworkDiscovery = m_NetworkManager.GetComponent<MyNetworkDiscovery>();
        m_NetworkDiscovery.StartClient();
        m_NetworkDiscovery.ClientBroadcast(new DiscoveryBroadcastData());
    }

    public void RefreshDiscovery()
    {
        m_MyNetworkDiscoveryHud.discoveredServers.Clear();
        m_NetworkDiscovery.ClientBroadcast(new DiscoveryBroadcastData());
        StartCoroutine(severScrollView.GetComponent<ServerScrollView>().GenButton());
    }

    public void Backgame()
    {
        m_NetworkDiscovery.StopDiscovery();
        m_MyNetworkDiscoveryHud.discoveredServers.Clear();
        SceneManager.LoadScene("MainMenu");
    }

}
