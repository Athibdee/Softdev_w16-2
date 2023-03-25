using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Girl_skill : NetworkBehaviour
{
    private NetworkObject local_girl;
    private AnimatedSprite skill_animated;
    private SpriteRenderer girl_renderer;
    public override void OnNetworkSpawn()
    {
        local_girl = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        girl_renderer = GetComponent<SpriteRenderer>();
        skill_animated = GetComponent<AnimatedSprite>();
    }


    

    [ServerRpc]
    public void Server_invisiblityServerRpc(ulong ObjectID_invisible_cast)
    {
        girl_renderer.enabled = false;
        Local_invisiblityClientRpc(ObjectID_invisible_cast);
        Invoke(nameof(invisibilityEndedClientRpc), 2f);
        //Debug.Log("test activated!");
    }

    [ClientRpc]
    public void Local_invisiblityClientRpc(ulong ObjectID_invisible_cast)
    {
        ulong local_girl_id = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().OwnerClientId;
        //Debug.Log($"pressed skill OwnerID:{ObjectID_invisible_cast} , OwnerClient_id is :{local_girl_id}");
        if (local_girl_id == ObjectID_invisible_cast)
        {
            skill_animated.IsSkill_idle = true;
            girl_renderer.enabled = true;
        }
        else
        {
            girl_renderer.enabled = false;
        }
    }

    [ClientRpc]
    public void invisibilityEndedClientRpc()
    {
        skill_animated.IsSkill_idle = false;
        girl_renderer.enabled = true;
    }

   
}

