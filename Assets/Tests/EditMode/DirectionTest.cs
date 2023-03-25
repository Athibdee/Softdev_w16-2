using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.EventSystems;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class DirectionTest
{
    private Joymovement movement;
    private GameObject player;
    NetworkManager m_NetworkManager;

    [OneTimeSetUp]
    public void SetUpNetworkManager()
    {
        GameObject NetworkManager = GameObject.FindWithTag("NetworkManager");
        if (NetworkManager == null)
        {
            SceneManager.LoadScene("StartUp");
        }
    }

    [UnitySetUp]
    public IEnumerator SetUpTest()
    {
        //Host game
        SceneManager.LoadScene("Host game");
        yield return null;

        GameObject canvas = GameObject.Find("Canvas");
        GameObject lobbyInterface = canvas.transform.Find("Button").gameObject;
        GameObject buttonGameObject = lobbyInterface.transform.Find("Host").gameObject;
        Button button = buttonGameObject.GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;

        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            yield return null;
        }
        movement = player.GetComponent<Joymovement>();
    }

    [UnityTearDown]
    public IEnumerator TearDownTest()
    {
        //If hosting then Shutdown
        yield return null;
        m_NetworkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        if (m_NetworkManager.IsHost)
        {
            m_NetworkManager.Shutdown();
        }
    }


}
