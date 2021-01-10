using EntityStates;
using EntityStates.Huntress;
using RoR2;
using RoR2.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkill : BaseSkillState
{
    public float damageCoefficient = .2f;
    public float baseDuration = 0.75f;
    public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerToolbotRebar");
    public AniviaAbilityHandler AniviaHandler;

    private float duration;
    private float fireDuration;
    private Animator animator;
    private string muzzleString;
    private float spellRadius;
    public Vector3 spellPosition;
    public Quaternion spellRotation;
    private int maxDuration = int.MaxValue;

    public override void OnEnter()
    {
        base.OnEnter();
        this.duration = this.baseDuration / this.attackSpeedStat;
        this.fireDuration = 0.25f * this.duration;
        this.spellRadius = 0.5f;
        base.characterBody.SetAimTimer(2f);
        base.characterBody.StartCoroutine(this.FireR());
        this.animator = base.GetModelAnimator();
        this.muzzleString = "Muzzle";
        base.PlayAnimation("Gesture, Override", "FireArrow", "FireArrow.playbackRate", this.duration);
        this.AniviaHandler = this.GetComponent<AniviaAbilityHandler>();
        Debug.Log("---------------------------------------------------------------Using Primary Ability!-------------------------------------------------------");
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private IEnumerator FireR()
    {
        yield return (object)new WaitForSeconds(this.fireDuration);


        if (this.AniviaHandler.hasR())
        {
            this.AniviaHandler.popR();
            this.outer.SetNextStateToMain();
        }
        else
        {
            this.AniviaHandler.createR(this.spellPosition, this.spellRotation, this.spellRadius);
            for (int i = 0; i < this.maxDuration; ++i)
            {
                this.AniviaHandler.updateIceZone();
                if (this.AniviaHandler.hasR())
                    yield return new WaitForFixedUpdate();
                else
                    break;
            }
            this.AniviaHandler.popR();
            this.outer.SetNextStateToMain();
        }

        /*
        if (base.isAuthority)
        {
            ProjectileManager.instance.FireProjectile(Anivia_Survivor.Anivia.PrimaryPrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
        }
        */
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override InterruptPriority GetMinimumInterruptPriority()
    {
        return InterruptPriority.PrioritySkill;
    }
}