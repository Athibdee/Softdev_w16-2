using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : NetworkBehaviour
{
    public SpriteRenderer spriteRedenderer { get; private set; }
    public Sprite[] sprites ;
    public Sprite[] skill_sprites;
    public Sprite[] WinerSprites;
    public float AnimationTime = 0.25f ;
    public int AnimationFrame { get; private set; }
    public int Skill_AnimationFrame { get; private set; }
    public int WinnerAnimationFrame { get; private set; }
    private GameManager_RPC gameManager_RPC;
    public bool loop = true;
    public bool IsSkill_idle = false;
    public bool IsWin = false;

    private void Awake()
    {
        this.spriteRedenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        gameManager_RPC = GameObject.FindWithTag("GameManager").GetComponent<GameManager_RPC>();
        InvokeRepeating(nameof(Advance), this.AnimationTime, this.AnimationTime);
    }

    private void Advance()
    {
        if (!this.spriteRedenderer.enabled )
        {
            return;
        }
        if (!IsSkill_idle && !IsWin) //normal idle
        {
            this.AnimationFrame++;
            
            if (this.AnimationFrame >= this.sprites.Length && this.loop)
            {
                this.AnimationFrame = 0;
            }


            if (this.AnimationFrame >= 0 && this.AnimationFrame < this.sprites.Length)
            {
                this.spriteRedenderer.sprite = this.sprites[this.AnimationFrame];
            }
        }
        else if (IsWin)
        {
            this.WinnerAnimationFrame++;
            if (this.WinnerAnimationFrame >= this.WinerSprites.Length && this.loop)
            {
                this.WinnerAnimationFrame = 0;
            }

            if (this.WinnerAnimationFrame >= 0 && this.WinnerAnimationFrame < this.WinerSprites.Length)
            {
                this.spriteRedenderer.sprite = this.WinerSprites[this.WinnerAnimationFrame];
            }
        }
        else
        {
            this.Skill_AnimationFrame++;
            if (this.Skill_AnimationFrame >= this.skill_sprites.Length && this.loop)
            {
                this.Skill_AnimationFrame = 0;
            }

            if (this.Skill_AnimationFrame >= 0 && this.Skill_AnimationFrame < this.skill_sprites.Length)
            {
                this.spriteRedenderer.sprite = this.skill_sprites[this.Skill_AnimationFrame];
            }
        }
        
    }

    public void SecondAnimated_activated()
    {
        this.AnimationFrame = -1 ;
        Advance();
    }

    [ServerRpc]
    public void Server_WinnerAnimationServerRpc()
    {
        Local_WinnerAnimationClientRpc();
        Invoke(nameof(WinnerAnimationEndedClientRpc), 4f);
    }

    [ClientRpc]
    public void Local_WinnerAnimationClientRpc()
    {
        IsWin = true;
    }

    [ClientRpc]
    private void WinnerAnimationEndedClientRpc()
    {
        IsWin = false;
    }
}
