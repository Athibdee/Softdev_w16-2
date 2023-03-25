using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using Pathfinding;

public class ItemManager : NetworkBehaviour
{
    //public GameObject botPrefab; // เลือก Prefab ของโพร์ฟาบอทที่จะใช้
    //public float spawnOffset = 4f; // ระยะห่างจากผู้เล่นเพื่อสร้างโพร์ฟาบอท
    private SpawnManager spawnManager;
    private GameManager_RPC gameManager_RPC;
    private ulong bot_target_id;

    public Girl_Rule Girl_rule;

    public enum Itemtype
    {
        IncressGhost,
        DecressGhost,
        StopGhost
    }

    public Itemtype type;


    public override void OnNetworkSpawn()
    {
        gameManager_RPC = GameObject.FindWithTag("GameManager").GetComponent<GameManager_RPC>();

    }

    private void Start()
    {
        Girl_rule = GetComponent<Girl_Rule>();
        gameManager_RPC = GameObject.FindWithTag("GameManager").GetComponent<GameManager_RPC>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            if (type == Itemtype.IncressGhost)
            {
                gameManager_RPC.SpawnBotServerRpc();

            }
            else if (type == Itemtype.DecressGhost)
            {
                gameManager_RPC.DeSpawnBotServerRpc();

            }
            else if (type == Itemtype.StopGhost)
            {
                BotstopServerRpc();
            }
            itemsetactiveServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void BotstopServerRpc()
    {
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
        foreach (GameObject bot in bots)
        {
            bot.GetComponent<AIPath>().enabled = false;
            
        }
        Invoke(nameof(BotStartAfter3Sec), 3f);
        BotstopClientRpc();
    }

    [ClientRpc]
    void BotstopClientRpc()
    {
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
        foreach (GameObject bot in bots)
        {
            bot.GetComponent<AIPath>().enabled = false;
        }
        Invoke(nameof(BotStartAfter3Sec), 3f);

    }

    void BotStartAfter3Sec()
    {
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
        foreach (GameObject bot in bots)
        {
            bot.GetComponent<AIPath>().enabled = true;
        }
    }




    [ClientRpc]
    public void itemsetactiveClientRpc()
    {
        gameObject.SetActive(false);

    }
    [ServerRpc(RequireOwnership = false)]
    public void itemsetactiveServerRpc()
    {
        gameObject.SetActive(false);
        itemsetactiveClientRpc();
    }
}
            
  
