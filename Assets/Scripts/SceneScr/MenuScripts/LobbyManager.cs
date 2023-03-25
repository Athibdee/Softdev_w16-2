using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    private NetworkObject playerLocal;
    private GameObject mainCamera, leavegame, playerskill, GhostLabel, HumanLabel;
    private GameManager_RPC gameManager_RPC;
    public int Prefabindex = 0 ;
    private skill_button_manager Skill_button_Manager;
    private SpawnManager spawnManager;
    private MyNetworkDiscovery myNetworkDiscovery;
    public int lobby_size = 5;

    private float InitialPositionX_min = 10f ;
    private float InitialPositionX_max = 12f;
    private float InitialPositionY_min = -37 ;
    private float InitialPositionY_max = -35;

  
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        gameManager_RPC = GameObject.FindWithTag("GameManager").GetComponent<GameManager_RPC>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        leavegame = GameObject.FindWithTag("LeaveGame");
        playerskill = GameObject.FindWithTag("PlayerSkill");
        GhostLabel = GameObject.FindWithTag("GhostLabel");
        HumanLabel = GameObject.FindWithTag("HumanLabel");

        spawnManager = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>();
        Skill_button_Manager = GameObject.FindWithTag("UI").GetComponent<skill_button_manager>();
        myNetworkDiscovery = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetworkDiscovery>();

        playerskill.SetActive(false);
        leavegame.SetActive(false);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);

        //Request To Server is Lobby fulled?
        IsLobbyFullServerRpc();
        Debug.Log($"This Connected Client id is {NetworkManager.LocalClient.ClientId}"); // ID !!

        //if Game not start , assign spawnManager to spawn initial prefab : human !!
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"Client Id {clientId} disconnected");

        // Remove the client human prefab index from dictionary.
        spawnManager.ClientHumanIndex.Remove(clientId);
    }

    [ServerRpc(RequireOwnership = false)]//Client request for get Prefab Index
    private void IsLobbyFullServerRpc()
    {
        ulong newArrival_id = NetworkManager.Singleton.ConnectedClientsIds[^1];

        if (NetworkManager.Singleton.ConnectedClientsIds.Count > lobby_size) // if full then kick 
        {
            NetworkManager.DisconnectClient(newArrival_id); // Disconnected id
            return;
        }
        //CanJoin
        if (!gameManager_RPC.IsgameStart)
        {
            spawnManager.SpawnHumanPrefabServerRpc(newArrival_id, Vector2.zero); // first connected Girl prefab
        }
    }


    //On Click Play Button !!!
    [ServerRpc]
    public void ChangePositionServerRpc()
    {
        Random_GhostServerRpc();
        Debug.Log($"ghost player id : {gameManager_RPC.ghost_id}");
        //spawnManager.DespawnAllclientServerRpc();
        NetworkObject ghost_player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(gameManager_RPC.ghost_id);
        //ghost_player.Despawn();
        
        //Start Gameplay
        StartGameClientRpc();
        StartCoroutine(GhostAppearedProcess(gameManager_RPC.ghost_id, new Vector2(11.47f, -19.39f)));

        foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(uid != gameManager_RPC.ghost_id) {
                //gameManager_RPC.DeadList.Add(uid);
                DeadListAdd_ClientRpc(uid);
                Debug.Log($"InDeadList : {uid}");
            }

                Vector3 new_position = new Vector3(Random.Range(InitialPositionX_min, InitialPositionX_max),
                    Random.Range(InitialPositionY_min, InitialPositionY_max) , -2 );
            
                NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).transform.position = new Vector2(new_position.x, new_position.y);
                //mainCamera.transform.position = new Vector3(11.47f, -19.39f , -10);
                //Change view camera
                mainCamera.GetComponent<Camera>().orthographicSize = 7.32f;
           
                ChangePositionClientRpc(uid, new_position);
        }
        Debug.Log("Changed Position!!");
    }

    [ClientRpc]
    public void DeadListAdd_ClientRpc(ulong uid) {
        gameManager_RPC.DeadList.Add(uid);
    }

    //Random 1 ghost among 5 players , ghost index setting 
    [ServerRpc]
    public void Random_GhostServerRpc() {
        int ghost_index = Random.Range(0,NetworkManager.Singleton.ConnectedClientsIds.Count);
        gameManager_RPC.ghost_id = NetworkManager.Singleton.ConnectedClientsIds[ghost_index]; //Server know ghost_id
        //gameManager_RPC.ghost_id = 1;
        //Setting skill and Prefab index ghost player
        GhostSettingClientRpc(gameManager_RPC.ghost_id);
    }

    //Change Client Position 
    [ClientRpc]
    public void ChangePositionClientRpc(ulong uid , Vector3 new_position)
    {
        if(uid == NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().OwnerClientId)
        {
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).transform.position = new Vector2(new_position.x, new_position.y);
            mainCamera.transform.position = new Vector3(11.47f, -19.39f, -10f);
            //Change view camera
            mainCamera.GetComponent<Camera>().orthographicSize = 7.32f;
            StartGameClientRpc();
        }
    }

    //Set Ghost Prefab index to client
    [ClientRpc]
    private void GhostSettingClientRpc(ulong ghost_id)
    {
        gameManager_RPC.DeadList.Clear();   
        Debug.Log($"NetworkManager.LocalClient.ClientId is :{NetworkManager.LocalClient.ClientId}");
        Debug.Log($"ghost_id is :{ghost_id}");
        gameManager_RPC.ghost_id= ghost_id;
        if (NetworkManager.LocalClient.ClientId == ghost_id)
        {
            Prefabindex = 1;// ghost
            Skill_button_Manager.Isghost = true; //Assign to client skill button
        }

        else {
            Prefabindex = 0;
            Skill_button_Manager.Isghost = false;
            //gameManager_RPC.DeadListAdd(ghost_id);// Player dont need to know only server need to know

        }
        //if not a ghost then add id to dead list
        //Debug.Log($"Index : {Prefabindex}");
    }


    //Show Winner Announcement
    public void ShowWinner(int winner_index) {
        if(winner_index == 1)//winner index = 1 is ghost win
        {
            HumanLabel.SetActive(false);
            GhostLabel.SetActive(true);
            ActivateWinnerAnimation(true);
        }
        else {
            GhostLabel.SetActive(false);
            HumanLabel.SetActive(true);
            ActivateWinnerAnimation(false);
        }
        gameObject.transform.GetChild(3).gameObject.SetActive(true);
        StartCoroutine(Dissapear_label());
    }

    IEnumerator Dissapear_label() {
        yield return new WaitForSeconds(4);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
    }

    private void ActivateWinnerAnimation(bool isGhostWin)
    {
        ulong uid = NetworkManager.Singleton.LocalClientId;
        if (!isGhostWin && (uid == gameManager_RPC.ghost_id))
        {
            return;
        }
        if (isGhostWin && !(uid == gameManager_RPC.ghost_id))
        {
            return;
        }
        GameObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject;
        player.GetComponent<AnimatedSprite>().Server_WinnerAnimationServerRpc();

    }

    //BackBTN
    public void OnclickBack()
    {
        leavegame.SetActive(true);
    }

    public void OnclickCancel()
    {
        leavegame.SetActive(false);
    }

    public void OnclickLeave()
    {
        Debug.Log("Leave the game");

        //Shutdown player : leave the game
        NetworkManager.Singleton.Shutdown();
        myNetworkDiscovery.StopDiscovery();

        //Reset Camera position and view
        mainCamera.transform.position = new Vector3(0f, -1.23f, -10f);
        mainCamera.GetComponent<Camera>().orthographicSize = 5.05f;
        //Reset UI
        WaitingGameClientRpc();

        SceneManager.LoadScene("MainMenu");
    }


    //when start/waiting player in game, Unnecessary UI in the game will disappear.
    [ClientRpc]
    public void WaitingGameClientRpc()
    {
        playerskill.SetActive(false);

        //when reset will show Start Btn
        gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        //gameManager_RPC.IsgameStart = true;
        playerskill.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);


    }

    IEnumerator GhostAppearedProcess(ulong ghost_id , Vector2 position)
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(3);
        spawnManager.DespawnPlayerServerRpc(ghost_id);
        spawnManager.SpawnPlayerPrefabServerRpc(ghost_id ,1, position); // first connected Girl prefab
        //Spawn ghost player Prefab with ghost client id and Prefab index

        gameManager_RPC.CountDownClientRpc(); //IsGameStart = True

    }

    [ClientRpc]
    private void TeleportAllPlayerClientRpc(Vector2 position)
    {
        NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.position = position;
    }

    [ClientRpc]
    private void MoveCameraAllPlayerClientRpc(Vector3 position, float orthographicSize)
    {
        mainCamera.transform.position = position;
        mainCamera.GetComponent<Camera>().orthographicSize = orthographicSize;
    }

    [ServerRpc]
    private void SpawnAllPlayerAsGirlServerRpc()
    {
        spawnManager.DespawnAllclientServerRpc();
        foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
        {
            spawnManager.SpawnHumanPrefabServerRpc(uid, Vector3.zero);
        }
    }

    [ServerRpc]
    public void OnGameEndServerRpc()
    {
        StartCoroutine(GoBackToLobby());
    }

    public IEnumerator GoBackToLobby()
    {
        yield return new WaitForSeconds(5);
        SpawnAllPlayerAsGirlServerRpc();
        TeleportAllPlayerClientRpc(Vector2.zero);
        MoveCameraAllPlayerClientRpc(new Vector3(0f, -1.23f, -10f), 5.05f);
        WaitingGameClientRpc();
    }
}