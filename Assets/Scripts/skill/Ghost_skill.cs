using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ghost_skill : NetworkBehaviour
{
    private NetworkObject local_ghost;
    private AnimatedSprite skill_animated;
    private SpriteRenderer ghost_renderer;
    private Joymovement ghost_movement;
    public override void OnNetworkSpawn()
    {
        local_ghost = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        ghost_renderer = GetComponent<SpriteRenderer>();
        skill_animated = GetComponent<AnimatedSprite>();
        ghost_movement = GetComponent<Joymovement>();

    }

    [ServerRpc]
    public void Server_SpeedUpServerRpc(ulong ObjectID_invisible_cast)
    {
        Local_SpeedUpClientRpc(ObjectID_invisible_cast);
        Invoke(nameof(SpeedUpEndedClientRpc), 2f);
        //Debug.Log("test activated!");
    }

    [ClientRpc]
    public void Local_SpeedUpClientRpc(ulong ObjectID_invisible_cast)
    {
        ulong local_ghost_id = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().OwnerClientId;
        Debug.Log($"pressed skill OwnerID:{ObjectID_invisible_cast} , OwnerClient_id is :{local_ghost_id}");
          skill_animated.IsSkill_idle = true;
          ghost_movement.Speed = 14;   
    }

    [ClientRpc]
    private void SpeedUpEndedClientRpc()
    {
        skill_animated.IsSkill_idle = false;
        ghost_movement.Speed = 8;

    }

    
}
