using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;



public class MovementTest
{
    private Joymovement joyMovement;
    public Movement pcMovement;
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
        joyMovement = player.GetComponent<Joymovement>();
        pcMovement = player.GetComponent<Movement>();
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

    //Functional_test
    [UnityTest]
    public IEnumerator Test_MoveObject_With_ArrowKeys()
    {
        // Save the initial position of the player
        Vector3 oldPlayerPosition = player.transform.position;

        joyMovement.enabled = false;
        pcMovement.enabled = true;

        yield return new WaitForSeconds(2f);

        //Click Down
        pcMovement.SetDirection(Vector2.down);

        yield return new WaitForSeconds(2f);

        //Check newposition less then oldposition
        Assert.Less(player.transform.position.y, oldPlayerPosition.y);

        //Click Right
        pcMovement.SetDirection(Vector2.right);

        yield return new WaitForSeconds(2f);

        //Check newposition less then oldposition
        Assert.Greater(player.transform.position.x, oldPlayerPosition.x);

        //Click Left
        pcMovement.SetDirection(Vector2.left);

        yield return new WaitForSeconds(2f);

        //Check newposition less then oldposition
        Assert.Less(player.transform.position.x, oldPlayerPosition.x);

        //Click Up
        pcMovement.SetDirection(Vector2.up);

        yield return new WaitForSeconds(2f);

        //Check newposition less then oldposition
        Assert.Greater(player.transform.position.y, oldPlayerPosition.y);


    }

    [UnityTest]
    public IEnumerator Test_pc_movement_enable()
    {
        joyMovement.enabled = false;
        Assert.IsFalse(joyMovement.enabled);
        yield return null;
        pcMovement.enabled = true;
        Assert.IsTrue(pcMovement.enabled);

    }


    //Unit Testing Joymovement has rigid
    [Test]
    public void Test_Joymovement_has_rigidbody2d()
    {
        //yield return new WaitForSeconds(0.1f);
        Assert.NotNull(joyMovement, "Movement exists");
        Assert.NotNull(joyMovement.GetComponent<Rigidbody2D>(), "Movement has Rigidbody2D");
    }

     // Unit Test
     //Testing Prefab change Position to North South West East

    [UnityTest]
    public IEnumerator Test_Move_Direction_to_North()
    {
        // Setup
        Vector2 Startposition = new Vector2(0f, 0f);
        var rb = player.GetComponent<Rigidbody2D>();

        // Test position equal startposition
        Assert.AreEqual(Startposition, rb.position);
        Vector2 targetPosition = Vector2.up;

        //Move Direction to North
        joyMovement.Direction = targetPosition;
        rb.MovePosition(rb.position + joyMovement.Direction);
        yield return new WaitForFixedUpdate();

        //Test targetposition equal last position
        Assert.AreEqual(targetPosition, joyMovement.Direction);
    }


    //Dont delete prototype
    //[UnityTest]
    //public IEnumerator Test_Move_Direction_to_North()
    //{
    //    var startPosition = player.transform.position;
    //    var rb = player.GetComponent<Rigidbody2D>();
    //    Vector2 targetPosition = new Vector2(0f, 1f);

    //    //Change Vector2 Direction to targetPosition
    //    movement.Direction = targetPosition;

    //    //Move Rigidbody
    //    rb.MovePosition(targetPosition);
    //    yield return new WaitForFixedUpdate();
    //    yield return new WaitForSeconds(1f);

    //    // Assert
    //    Assert.AreEqual(targetPosition, movement.Direction);
    //}

    [UnityTest]
    public IEnumerator Test_move_direction_South()
    {
        // Setup
        Vector2 Startposition = new Vector2(0f, 0f);
        var rb = player.GetComponent<Rigidbody2D>();

        // Test position equal startposition
        Assert.AreEqual(Startposition, rb.position);
        Vector2 targetPosition = Vector2.down;

        //Move Direction to South
        joyMovement.Direction = targetPosition;
        rb.MovePosition(rb.position + joyMovement.Direction);
        yield return new WaitForFixedUpdate();

        //Test targetposition equal last position
        Assert.AreEqual(targetPosition, joyMovement.Direction);

    }

    [UnityTest]
    public IEnumerator Test_move_direction_East()
    {
        // Setup
        Vector2 Startposition = new Vector2(0f, 0f);
        var rb = player.GetComponent<Rigidbody2D>();

        // Test position equal startposition
        Assert.AreEqual(Startposition, rb.position);
        Vector2 targetPosition = Vector2.left;

        //Move Direction to East
        joyMovement.Direction = targetPosition;
        rb.MovePosition(rb.position + joyMovement.Direction);
        yield return new WaitForFixedUpdate();

        //Test targetposition equal last position
        Assert.AreEqual(targetPosition, joyMovement.Direction);

    }

    [UnityTest]
    public IEnumerator Test_move_direction_West()
    {
            // Setup
            Vector2 Startposition = new Vector2(0f, 0f);
            var rb = player.GetComponent<Rigidbody2D>();

            // Test position equal startposition
            Assert.AreEqual(Startposition, rb.position);
            Vector2 targetPosition = Vector2.right;

            //Move Direction to West
            joyMovement.Direction = targetPosition;
            rb.MovePosition(rb.position + joyMovement.Direction);
            yield return new WaitForFixedUpdate();

            //Test targetposition equal last position
            Assert.AreEqual(targetPosition, joyMovement.Direction);

    }
}
