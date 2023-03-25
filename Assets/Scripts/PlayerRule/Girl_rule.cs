using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Pathfinding;
public class Girl_rule : NetworkBehaviour
{
    public bool Islife = true;
    private GameManager_RPC gameManager_RPC;
    public LayerMask GhostLayer;
    //public Transform Target_transform;



    public override void OnNetworkSpawn()
    {
        gameManager_RPC = GameObject.FindWithTag("GameManager").GetComponent<GameManager_RPC>();

        //request_target_transformServerRpc(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().OwnerClientId);
    }



    //[ServerRpc(RequireOwnership = false)]
    //private void request_target_transformServerRpc(ulong client_id)
    //{
    //    ulong target_index = gameManager_RPC.DeadList[0];
    //    GameObject target_Object = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(target_index).gameObject;
    //    Transform target_transform = target_Object.transform;
    //    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(client_id).gameObject.GetComponent<AIDestinationSetter>().target = target_transform;
    //}



}