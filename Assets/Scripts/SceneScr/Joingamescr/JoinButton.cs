using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class JoinButton : MonoBehaviour
{
    public ServerScrollView serverScrollView;
    KeyValuePair<IPAddress, MyDiscoveryResponseData> m_discoveredServer;

    public void SetDataConnection(KeyValuePair<IPAddress, MyDiscoveryResponseData> discoveredServer)
    {
        m_discoveredServer = discoveredServer;

        string serverName = discoveredServer.Value.ServerName;
        string IP = discoveredServer.Key.ToString();
        int playerCount = discoveredServer.Value.PlayerCount;
        string text = $"{serverName}[{IP}]({playerCount}/5)";
        gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().fontSize = 28;
        gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
        gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = text;
    }

    public void Button_Click()
    {
        serverScrollView.ButtonClicked(m_discoveredServer);
    }

}
