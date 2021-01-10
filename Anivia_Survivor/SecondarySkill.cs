using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondarySkill : BaseSkillState
{
    public float damageCoefficient = 2f;
    public float baseDuration = 0.75f;
    public float recoil = 1f;
    public float speed = 10f;
    public float maxDistance = 100f;
    public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerToolbotRebar");
    public AniviaAbilityHandler AniviaHandler;

    private float duration;
    private float fireDuration;
    private bool maxRange;
    private Animator animator;
    private string muzzleString;

    public override void OnEnter()
    {
        base.OnEnter();
        this.duration = this.baseDuration / this.attackSpeedStat;
        this.fireDuration = 0.25f * this.duration;
        base.characterBody.SetAimTimer(2f);
        base.characterBody.StartCoroutine(this.FireQ());
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

    private IEnumerator FireQ()
    {
        Debug.Log("I'm waiting!");
        yield return (object)new WaitForSeconds(this.fireDuration);

        base.characterBody.AddSpreadBloom(0.75f);
        Ray aimRay = base.GetAimRay();

        if (this.AniviaHandler.hasQ())
        {
            this.AniviaHandler.popQ();
        }
        else
        {
            this.AniviaHandler.createQ(aimRay);
            for (int i = 0; i < 1000; ++i)
            {
                this.AniviaHandler.updateIceBall(this.speed);
                if (this.AniviaHandler.hasQ() && (this.AniviaHandler.iceBallTravelDistance() < this.maxDistance))
                    yield return new WaitForFixedUpdate();
                else
                    break;
            }
            this.AniviaHandler.popQ();
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

        if ((base.fixedAge >= this.duration && base.isAuthority) || (this.maxRange && base.isAuthority))
        {
            this.outer.SetNextStateToMain();
        }
    }

    public override InterruptPriority GetMinimumInterruptPriority()
    {
        return InterruptPriority.Skill;
    }
}
