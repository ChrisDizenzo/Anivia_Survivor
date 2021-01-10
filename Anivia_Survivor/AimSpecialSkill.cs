using EntityStates;
using EntityStates.Huntress;
using RoR2;
using RoR2.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimSpecialSkill : BaseSkillState
{
    public float baseDuration = 0.75f;
    public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerToolbotRebar");
    public AniviaAbilityHandler AniviaHandler;

    private float duration;
    private Animator animator;
    private float spellRadius;
    private GameObject aimArea;
    private GameObject defaultCrosshairPrefab;
    private float maxDistance = 100f;

    public override void OnEnter()
    {
        base.OnEnter();
        this.spellRadius = 16f;
        this.animator = base.GetModelAnimator();
        //base.PlayAnimation("Gesture, Override", "FireArrow", "FireArrow.playbackRate", this.duration);
        this.AniviaHandler = this.GetComponent<AniviaAbilityHandler>();
        Debug.Log("---------------------------------------------------------------Aiming Special Ability!-------------------------------------------------------");
        if (this.AniviaHandler.hasR())
        {
            this.AniviaHandler.popR();
            this.outer.SetNextStateToMain();
        }
        this.defaultCrosshairPrefab = this.characterBody.crosshairPrefab;
        if (this.cameraTargetParams)
            this.cameraTargetParams.aimMode = CameraTargetParams.AimType.AimThrow;
        if (!ArrowRain.areaIndicatorPrefab)
            return;
        this.aimArea = Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
        this.aimArea.transform.localScale = new Vector3(this.spellRadius, this.spellRadius, this.spellRadius);
        
    }

    public override void OnExit()
    {
        this.characterBody.hideCrosshair = false;
        if (this.aimArea)
        {
            EntityState.Destroy(this.aimArea.gameObject);
        }
        if (this.cameraTargetParams)
        {
            this.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
        }
        base.OnExit();
    }

    private void updateAimArea()
    {
        if (!this.aimArea)
            return;
        Ray aimRay = this.GetAimRay();
        RaycastHit hitInfo;
        if (Physics.Raycast(aimRay, out hitInfo, this.maxDistance, (int)LayerIndex.CommonMasks.bullet))
        {
            this.aimArea.transform.position = hitInfo.point;
            //this.aimArea.transform.up = hitInfo.normal;
        }
        else
        {
            this.aimArea.transform.position = aimRay.GetPoint(this.maxDistance);
            //this.aimArea.transform.up = -aimRay.direction;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!this.isAuthority || this.IsKeyDownAuthority())
            return;
        this.StartAimMode(0.5f);
        SpecialSkill nextState = new SpecialSkill();
        nextState.spellPosition = this.aimArea.transform.position;
        nextState.spellRotation = this.aimArea.transform.rotation;
        this.outer.SetNextState(nextState);
    }
    public override void Update()
    {
        base.FixedUpdate();
        this.updateAimArea();
    }

    public override InterruptPriority GetMinimumInterruptPriority()
    {
        return InterruptPriority.PrioritySkill;
    }
}