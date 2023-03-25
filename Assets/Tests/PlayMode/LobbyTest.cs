using System.Collections;
using NUnit.Framework;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class LobbyTest
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

    [OneTimeTearDown]
    public void TearDownNetworkManager()
    {
        if (m_NetworkManager.IsHost)
        {
            m_NetworkManager.Shutdown();
        }
    }

    void FindButtonAndInvoke(string buttonName)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject lobbyInterface = canvas.transform.Find("Lobby").gameObject;
        GameObject buttonGameObject = lobbyInterface.transform.Find(buttonName).gameObject;
        Button button = buttonGameObject.GetComponent<Button>();
        button.onClick.Invoke();
    }

    // Model created a host and entered his lobby. He saw his female character.
    [UnityTest]
    public IEnumerator Test_when_player_host_will_create_character_in_lobby()
    {

        //Model Host and join to and he saw his female character
        GameObject player = GameObject.FindWithTag("Player");
        Assert.IsNotNull(player, "The player character is not found");
        yield return null;
    }

    // Model Want to leave game/SampleScene and Go to main menu 
    [UnityTest]
    public IEnumerator Test_when_leave_lobby_then_popup_alert_and_leave()
    {
        //Model Click Back BTN
        FindButtonAndInvoke("Back");
        yield return null;

        //Model see Popup UI
        GameObject leaveGame = GameObject.FindWithTag("LeaveGame");
        Assert.IsTrue(leaveGame.activeSelf, "The LeaveGame UI is not shown");


        //Model Click Leave button for leave the game
        GameObject leaveBtn = leaveGame.transform.Find("LeaveBtn").gameObject;
        Button btn = leaveBtn.GetComponent<Button>();
        btn.onClick.Invoke();
        yield return null;

        //Model back to MainMenu Scene
        Scene currentScene = SceneManager.GetActiveScene();
        Assert.AreEqual("MainMenu", currentScene.name
            , "The scene is not change to \"MainMenu\" when click Leave Button");

    }

    // Mr. Model wants to leave the game. But after thinking about it again,
    // it's better to continue playing, so I press cancel.
    [UnityTest]
    public IEnumerator Test_when_leave_lobby_but_you_want_to_cancel()
    {
        //Model Click Back Button
        FindButtonAndInvoke("Back");
        yield return null;

        //Model see Popup UI
        GameObject leaveGame = GameObject.FindWithTag("LeaveGame");
        Assert.IsTrue(leaveGame.activeSelf, "The LeaveGame UI is not shown");

        //Model Click Cancel from UI
        GameObject leaveBtn = leaveGame.transform.Find("CancelBtn").gameObject;
        Button Btn = leaveBtn.GetComponent<Button>();
        Btn.onClick.Invoke();
        yield return null;


        //Model back to Game (SampleScene)
        Scene currentScene = SceneManager.GetActiveScene();
        Assert.AreEqual("SampleScene", currentScene.name,
            "The scene is not \"SampleScene\" when click Cancel Button");

    }

    //Model want to play game , He click Start Button to Start the game
    //Model and everyone in lobby change position to Game map
    [UnityTest]
    public IEnumerator Test_when_start_game_change_position()
    {

        //Model Click Start Button
        FindButtonAndInvoke("Start");
        yield return null;

        //Model see skill button and can't see play button (Because when game starts, the button will disappears.)
        GameObject PlayerSkill = GameObject.FindWithTag("PlayerSkill");
        Assert.IsTrue(PlayerSkill.activeSelf);

        GameObject Lobby = GameObject.FindWithTag("LobbyUI");
        GameObject playBtn = Lobby.transform.GetChild(0).gameObject;
        Assert.IsFalse(playBtn.activeSelf);

        //Model's character is change position to game-map
        float xMin = 9.4f;
        float xMax = 11.5f;
        float yMin = -19.5f;
        float yMax = -17.6f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = player.transform.position;

        Assert.GreaterOrEqual(playerPos.x, xMin);
        Assert.LessOrEqual(playerPos.x, xMax);
        Assert.GreaterOrEqual(playerPos.y, yMin);
        Assert.LessOrEqual(playerPos.y, yMax);

        //Model's MainCamera view will change position and increase point of view
        Camera camera = Camera.main; ;
        Vector3 cameraTrans = new Vector3(11.47f, -19.39f, -10f);
        Assert.AreEqual(cameraTrans, camera.transform.position);
        Assert.AreEqual(7.32, camera.orthographicSize, 0.1f);
    }

    [UnityTest]
    public IEnumerator Test_after_3sec_wolf_spawn()
    {
        yield return new WaitForSeconds(1f);
        FindButtonAndInvoke("Start");
        yield return new WaitForSeconds(3f);

        GameObject wolf = GameObject.FindWithTag("Wolf");
        Assert.IsNotNull(wolf, "wolf character is not found");
        yield return null;


    }
}