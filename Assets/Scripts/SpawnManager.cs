using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;

public class SpawnManager : NetworkBehaviour
{

    //[SerializeField] private GameObject playerPrefabA; //add prefab in inspector
    //[SerializeField] private GameObject playerPrefabB; //add prefab in inspector

    //private bool Activatespawn = true;
    

    [SerializeField] private List<GameObject> Network_Prefab_List;
    [SerializeField] private List<GameObject> HumanPrefabList;
    public Dictionary<ulong,int> ClientHumanIndex = new Dictionary<ulong, int>();

    private GameManager_RPC gameManagerRPC; //using only server side 
    //public LobbyManager lobby;

    ////when new connected is successful the network spawn be executed
    public override void OnNetworkSpawn()
    {
        //success Connected
        gameManagerRPC = GameObject.FindWithTag("GameManager").GetComponent<GameManager_RPC>();
        //lobby = GameObject.FindWithTag("Canvas").GetComponentInChildren<LobbyManager>();
    }

    private int GetAvailableHumanPrefabIndex()
    {
        int Prefabindex = 0;
        while (ClientHumanIndex.ContainsValue(Prefabindex))
        {
            Prefabindex++;
        }
        return Prefabindex;
    }

    //Spawn Game Player Object with Prefab index that setting by LobbyManager
    [ServerRpc(RequireOwnership = false)]//server owns this object but client can request a spawn
    public void SpawnPlayerPrefabServerRpc(ulong client_id, int Prefabindex ,Vector2 position, ServerRpcParams serverRpcParams = default)
    {
        GameObject gameObject = (GameObject)Instantiate(Network_Prefab_List[Prefabindex]); // note : need to use a clone from prefab by using Instantitiate!!
        gameObject.SetActive(true);
        gameObject.transform.position = position;
        gameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(client_id); // Beware!! this one have latency
        
        Debug.Log($"FROM SpawnManager : ServerClientID{client_id}: spawned!");
    }

    // ----For Human----
    //Spawn Game Player Object with Prefab index that setting by LobbyManager
    [ServerRpc(RequireOwnership = false)]//server owns this object but client can request a spawn
    public void SpawnHumanPrefabServerRpc(ulong client_id, Vector2 position, ServerRpcParams serverRpcParams = default)
    {
        // If the client already have a human
        int Prefabindex;
        if (ClientHumanIndex.ContainsKey(client_id))
        {
            Prefabindex = ClientHumanIndex[client_id];
        }
        else
        {
            // If not, Get available human prefab index
            Prefabindex = GetAvailableHumanPrefabIndex();
            ClientHumanIndex[client_id] = Prefabindex;
        }

        GameObject gameObject = (GameObject)Instantiate(HumanPrefabList[Prefabindex]); // note : need to use a clone from prefab by using Instantitiate!!
        gameObject.SetActive(true);
        gameObject.transform.position = position;
        gameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(client_id); // Beware!! this one have latency

        Debug.Log($"FROM SpawnManager : ServerClientID{client_id}: spawn as {HumanPrefabList[Prefabindex].name}!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnBotGhostServerRpc(int Prefabindex, Vector2 position, ServerRpcParams serverRpcParams = default)
    {
        // Instantiate the prefab
        GameObject gameObject = (GameObject)Instantiate(Network_Prefab_List[Prefabindex]);
        gameObject.SetActive(true);
        gameObject.transform.position = position;
        gameObject.GetComponent<NetworkObject>().Spawn(true); //.SpawnAsPlayerObject(client_id); // Spawn without client ownership
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnBotServerRpc( ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"Despawn Bot!!");
        var bots = GameObject.FindGameObjectsWithTag("Bot");
        if (bots.Length > 0)
        {
            // chooise bot spawn first and despawn
            var bot = bots[0];
            bot.GetComponent<NetworkObject>().Despawn(true);
        }
    }



    //Despawn All Client Object
    [ServerRpc]
    public void DespawnAllclientServerRpc()
    {
        foreach (NetworkClient Client_obj in NetworkManager.Singleton.ConnectedClientsList)
        {
            Client_obj.PlayerObject.Despawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnPlayerServerRpc(ulong target_id, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"Despawn Player: {target_id}!!");
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(target_id).Despawn(true);
    }

}
