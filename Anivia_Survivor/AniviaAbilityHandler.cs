using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Huntress;

public class AniviaAbilityHandler : MonoBehaviour
{
    private CharacterDirection direction;
    private CharacterBody body;
    private NetworkIdentity thisInstance;
    private NetworkUser thisUser;

    private GameObject iceBall;
    private Ray aimRay;

    private GameObject iceAimZone;
    private GameObject iceZone;
    private float iceZoneRadius;
    private float maxiceZoneRadius = 3f;

    public void Awake()
    {
        this.body = this.gameObject.GetComponent<CharacterBody>();
        this.thisInstance = this.gameObject.GetComponent<NetworkIdentity>();
        this.thisUser = this.gameObject.GetComponent<NetworkUser>();
        this.direction = this.gameObject.GetComponent<CharacterDirection>();
        Debug.Log("---------------------------------------------------------------Defining Anivia Handler!-------------------------------------------------------");
        foreach (Component c in this.gameObject.GetComponents<Component>())
        {
            Debug.Log(c.GetType());
        }
    }

    public bool hasQ()
    {
        return iceBall != null;
    }

    public bool hasR()
    {
        return iceZone != null;
    }
    public bool aimingR()
    {
        return iceZone != null;
    }

    public void createR(Ray r)
    {
        aimRay = r;
        iceBall = UnityEngine.Object.Instantiate<GameObject>(Assets.primarySkill);
        iceBall.transform.position = this.transform.position;
    }

    public void aimR(Ray r, float spellRadius)
    {
        aimRay = r;

        if (!(bool)(Object)ArrowRain.areaIndicatorPrefab)
            return;
        this.iceAimZone = Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
        this.iceAimZone.transform.localScale = new Vector3(spellRadius, spellRadius, spellRadius);
        this.iceZoneRadius = spellRadius;
    }

    public void createQ(Ray r)
    {
        aimRay = r;
        iceBall = UnityEngine.Object.Instantiate<GameObject>(Anivia_Survivor.Anivia.PrimaryPrefab);
        iceBall.transform.position = this.transform.position;
    }

    public void createR(Vector3 spellPosition, Quaternion spellRotation, float spellRadius)
    {
        iceZone = UnityEngine.Object.Instantiate<GameObject>(Assets.specialSkill);
        iceZone.transform.position = spellPosition;
        iceZone.transform.rotation = spellRotation;
        iceZone.transform.localScale = new Vector3(spellRadius, spellRadius, spellRadius);
    }

    public void updateIceBall(float speed)
    {
        if (hasQ())
        {
            BlastAttack blastAttack = new BlastAttack()
            {
                radius = 10f,
                procCoefficient = 1f,
                position = iceBall.transform.position,
                attacker = this.gameObject,
                crit = Util.CheckRoll(this.body.crit, this.body.master),
                baseDamage = 0f,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.SlowOnHit,
                baseForce = 0f,
            };
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.Fire();
            iceBall.transform.position += aimRay.direction * Time.fixedDeltaTime * speed;
        }
    }

    public void updateIceZone()
    {
        if (hasR())
        {
            float damage = this.body.damage / 10;
            if (this.iceZoneRadius < maxiceZoneRadius)
            {
                this.iceZoneRadius += 0.001f;
            } else 
            {
                damage = this.body.damage * 2 ;
            }
            iceZone.transform.localScale = new Vector3(this.iceZoneRadius, this.iceZoneRadius, this.iceZoneRadius);
            BlastAttack blastAttack = new BlastAttack()
            {
                radius = this.iceZoneRadius,
                procCoefficient = 1f,
                position = this.iceZone.transform.position,
                attacker = this.gameObject,
                crit = Util.CheckRoll(this.body.crit, this.body.master),
                baseDamage = damage,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.SlowOnHit,
                baseForce = 0f,
            };
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.Fire();
            //iceZone.transform.localScale = new Vector3();
            //iceZone.transform.up
        }
    }

    public float iceBallTravelDistance()
    {
        return Vector3.Distance(aimRay.origin, iceBall.transform.position);
    }

    public void popQ()
    {
        if (hasQ())
        {
            BlastAttack blastAttack = new BlastAttack()
            {
                radius = this.iceZoneRadius,
                procCoefficient = 1f,
                position = iceBall.transform.position,
                attacker = this.gameObject,
                crit = Util.CheckRoll(this.body.crit, this.body.master),
                baseDamage = this.body.damage,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.Freeze2s,
                baseForce = 20f,
            };
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.Fire();
            UnityEngine.Object.Destroy(iceBall);
            iceBall = null;
        }

    }
    public void popR()
    {
        if (hasR())
        {
            BlastAttack blastAttack = new BlastAttack()
            {
                radius = 10f,
                procCoefficient = 1f,
                position = this.iceZone.transform.position,
                attacker = this.gameObject,
                crit = Util.CheckRoll(this.body.crit, this.body.master),
                baseDamage = this.body.damage,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.SlowOnHit,
                baseForce = 0f,
            };
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.Fire();
            UnityEngine.Object.Destroy(iceZone);
            iceBall = null;
        }
    }
}
