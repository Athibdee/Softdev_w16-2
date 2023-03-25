using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class skill_button_manager : NetworkBehaviour
{
    public Girl_skill girl_skill ;
    public Ghost_skill ghost_skill;

    public AnimatedSprite Skill_Animated;
    //public ulong local_girl_OwnerClientID;
    public NetworkObject local_player;
    private float counter = 0 ;
    private bool Isinvisible = false;
    public bool Isghost = true;
    [SerializeField] private AudioSource GhostSkillEffect;
    [SerializeField] private AudioSource HumanSkillEffect;
    public void Get_local_skill_script()
    {
        local_player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (Isghost)
        {
            ghost_skill = local_player.GetComponent<Ghost_skill>();

        }
        else
        {
            girl_skill = local_player.GetComponent<Girl_skill>();

        }
        
    }

    

    public void Skill_cast()
    {
        //Debug.Log("Activated!!");
        Get_local_skill_script();

        if (!Isinvisible && !Isghost)
        {
            HumanSkillEffect.Play();
            Isinvisible = true;
            girl_skill.Server_invisiblityServerRpc(local_player.OwnerClientId); // Invisibility skill (completed!)
        }

        if(!Isinvisible && Isghost)
        {
            GhostSkillEffect.Play();
            Isinvisible = true;
            ghost_skill.Server_SpeedUpServerRpc(local_player.OwnerClientId); //Speed skill
        }
        
    }

    public void FixedUpdate()
    {
        if (counter <= 5f && Isinvisible)
        {
            counter += Time.deltaTime;
        }
        else
        {
            counter = 0;
            Isinvisible = false;
            

        }
    }


}
