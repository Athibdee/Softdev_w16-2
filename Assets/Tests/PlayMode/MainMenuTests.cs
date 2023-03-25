using NUnit.Framework;
using System.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MainMenuTests
{
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

    [SetUp]
    public void SetUpTest()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void FindButtonAndInvoke(string buttonName)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject menuSystem = canvas.transform.Find("Menu system").gameObject;
        GameObject buttonGameObject = menuSystem.transform.Find(buttonName).gameObject;

        Button button = buttonGameObject.GetComponent<Button>();
        button.onClick.Invoke();
    }

    [UnityTest]
    public IEnumerator Test_can_change_scene_when_click_find_game()
    {
        FindButtonAndInvoke("Find Game");
        yield return null;

        Scene currentScene = SceneManager.GetActiveScene();
        Assert.AreEqual("Join game", currentScene.name,
            "The scene isn't change to \"Join game\" scene when click Find Game button");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_can_change_scene_when_click_host_game()
    {
        FindButtonAndInvoke("Host Game");
        yield return null;

        Scene currentScene = SceneManager.GetActiveScene();
        Assert.AreEqual("Host game", currentScene.name,
            "The scene isn't change to \"Host game\" scene when click Host Game button");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_can_change_scene_when_click_settings()
    {
        FindButtonAndInvoke("Settings");
        yield return null;

        Scene currentScene = SceneManager.GetActiveScene();
        Assert.AreEqual("Settings", currentScene.name,
            "The scene isn't change to \"Settings\" scene when click Settings button");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_can_change_scene_when_click_credits()
    {
        FindButtonAndInvoke("Credits");
        yield return null;

        Scene currentScene = SceneManager.GetActiveScene();
        Assert.AreEqual("IntroScene", currentScene.name,
            "The scene isn't change to \"IntroScene\" when click Credits button");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_can_find_lobby_and_human_win()
    {
        FindButtonAndInvoke("Find Game");
        yield return new WaitForSeconds(1f);

        GameObject leaveGame = GameObject.FindWithTag("LeaveGame");

        //Click Button in Serverlist
        GameObject canvas = GameObject.Find("Canvas");
        GameObject Serverlist = canvas.transform.Find("Server list").gameObject;
        GameObject view = Serverlist.transform.Find("Viewport").gameObject;
        GameObject content = view.transform.Find("Content").gameObject;
        GameObject serverBtn = content.transform.Find("JoinButton(Clone)").gameObject;
        Button btn = serverBtn.GetComponent<Button>();
        btn.onClick.Invoke();

        //wait human playerwin
        yield return new WaitForSeconds(28f);

        //check humanwin label show
        GameObject GhostWin = GameObject.FindWithTag("GhostLabel");
        GameObject HumanWin = GameObject.FindWithTag("HumanLabel");
        Assert.IsFalse(GhostWin.activeSelf);
        Assert.IsTrue(HumanWin.activeSelf);

    }
}
