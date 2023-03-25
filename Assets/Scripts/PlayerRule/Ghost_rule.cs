using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Ghost_rule : NetworkBehaviour
{
    private float time = 0f;
    private GameManager_RPC gameManager_RPC;
    public LayerMask GirlLayer;
    private ulong target_girl_id;
    public bool Activated_kill = false;
    public float HitBox_size = 0.75f;
    [SerializeField] private AudioSource GhostKillEffect;
    public void Start()
    {
        gameManager_RPC = GameObject.FindWithTag("GameManager").GetComponent<GameManager_RPC>();
        target_girl_id = 999;
    }

    public void Update()
    {
        if (IsHit_Girl() && Activated_kill)
        {
            GhostKillEffect.Play();
            gameManager_RPC.GhostKillGirlServerRpc(target_girl_id);
            Debug.Log("Hit!!");
        }
    }

    public bool IsHit_Girl()
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * HitBox_size, 0.0f,Vector2.one, 0, this.GirlLayer);
        if (hit.collider != null)//.collider = null if in that boxcast is not collise with obstacleLayer
        {
            target_girl_id = hit.collider.gameObject.GetComponent<NetworkObject>().OwnerClientId; 
        }
        return hit.collider != null; //.collider = null if in that boxcast is not collise with obstacleLayer
    }

    public void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time >= 3f && !Activated_kill) // for first 3 second
        {
            Debug.Log("Ghost Acitvated kill");
            Activated_kill = true;
        }
    }
}
