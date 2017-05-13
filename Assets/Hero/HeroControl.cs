using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeroControl : MOBAUnit {

    //FIELDS

    //private fields
    [SerializeField]
    private float walkSpeed = 1.5f;
    private float runSpeed = 3f;
    private Transform _moveDestination;
    private MOBAUnit _attackTarget;
    private float _attackDelay = 1f;
    private float _attackedLastTime;

    [SerializeField]
    private float closeEnoughForMove = 0.4f;
    [SerializeField]
    private float closeEnoughForAttack = 1f;
    
    [SerializeField]
    private float _maxMana;
    private float _curMana;

    public List<BasicAbility> MyList = new List<BasicAbility>();

    private BaseControl _homeBase;

    [SerializeField]
    private HearthStoneAbility HearthStone;
    [SerializeField]
    private UntargettedAbility Ability1;
    [SerializeField]
    private TargettedAbility Ability2;

    //will be used to remember what status to return to, after casting a spell
    private MOBAUnit.UnitStatus _lastStatusBeforeCasting;

	// Use this for initialization
	override protected void Start () {
        base.Start();
        _curMana = 0;
        _attackedLastTime = 0;
	}

	/**
     * This public method is used by the GUI, the AI or the network to make the hero do something
     */
    public void MoveTo(Transform target, bool running = false)
    {
        //first check illegal state transitions (are there any?)

        //then change status:
        _moveDestination = target;
        if (running)
        {
            SetStatus(UnitStatus.RUNNING);
        }
        else
        {
            SetStatus(UnitStatus.WALKING);
        }
        Debug.Log("I'm moving to " + target.name);
    }

    /**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
     */
    public void ActivateHearthStone()
    {
        //first check legal status? Ability will check correct timing
        if (this.HearthStone.Activate())
        {
            SetStatus(UnitStatus.ABILITY1);
        }
    }
    public float GetHearthStoneCoolDown()
    {
        return HearthStone.GetCurrentCoolDownTime();
    }

    /**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
     */
    public void ActivateAbility1()
    {
        //first check legal status? Ability will check correct timing
        if (this.Ability1.Activate())
        {
            _lastStatusBeforeCasting = GetStatus();
            SetStatus(UnitStatus.ABILITY1);
        }
    }
    public float GetAbility1CoolDown()
    {
        return Ability1.GetCurrentCoolDownTime();
    }

    /**
     * Tells the GUI what the "valid" targets are for ability2
     */
    public int GetAbility2Mask()
    {
        return Ability2.GetValidTargetMask();
    }
    public float GetAbility2CoolDown()
    {
        return Ability2.GetCurrentCoolDownTime();
    }

    /**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
     */
    public void ActivateAbility2(GameObject target)
    {
        //first check legal status? Ability will check correct timing
        if (this.Ability2.Activate(target))
        {
            _lastStatusBeforeCasting = GetStatus();
            SetStatus(UnitStatus.ABILITY2);
        }
    }

    protected override void UpdateIdle()
    {
        UpdateAllSkills();
    }

    protected override void UpdateWalking()
    {
        UpdateMove(walkSpeed);
    }

    protected override void UpdateRunning()
    {
        UpdateMove(runSpeed);
    }

    /**
     * Possible transitions:
     * -> IDLE (when destination has been reached)
     */
    private void UpdateMove(float speed)
    {
        UpdateAllSkills();

        Rotate2DTo(_moveDestination);
        float distSqr = (transform.position - _moveDestination.position).sqrMagnitude;
        if (distSqr < closeEnoughForMove)
        {
            SetStatus(UnitStatus.IDLE);
            Debug.Log("I've reached my destination");
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private void Rotate2DTo(Transform target)
    {
        Vector3 dest = target.position;
        dest.y = transform.position.y; //set destination at same height to avoid pitch change
        transform.LookAt(dest);
    }

    /**
     * public method used by GUI, AI, or network to make the hero do something
     */
    public void Attack(MOBAUnit enemy)
    {
        //if necessary, check for illegal state transitions

        _attackTarget = enemy;
        if ((enemy.transform.position - transform.position).sqrMagnitude > closeEnoughForAttack)
        {
            SetStatus(UnitStatus.ATTACKRUNNING);
        }
        else
        {
            SetStatus(UnitStatus.ATTACKING);
        }
    }

    protected override void UpdateAttackRunning()
    {
        UpdateAllSkills();

        Rotate2DTo(_attackTarget.transform);
        float distSqr = (transform.position - _attackTarget.transform.position).sqrMagnitude;
        if (distSqr < closeEnoughForAttack)
        {
            SetStatus(UnitStatus.ATTACKING);
        }
        else
        {
            transform.position += transform.forward * runSpeed * Time.deltaTime;
        }
    }

    protected override void UpdateAttacking()
    {
        UpdateAllSkills();

        if (Time.time - _attackedLastTime > _attackDelay)
        {
            _attackedLastTime = Time.time;
        }
    }

    protected override void UpdateHearthStone()
    {
        UpdateAllSkills();
        //when ability is done casting, hero should become status "idle"
        if (HearthStone.GetStatus() != BasicAbility.SkillStatus.CASTING) SetStatus(MOBAUnit.UnitStatus.IDLE);
    }

    protected override void UpdateAbility1()
    {
        UpdateAllSkills();
        //when ability is done casting, hero should become whatever status before the casting (usually attacking or idle)
        if (Ability1.GetStatus() != BasicAbility.SkillStatus.CASTING) SetStatus(_lastStatusBeforeCasting);
    }
    protected override void UpdateAbility2()
    {
        UpdateAllSkills();
        //when ability is done casting, hero should become whatever status before the casting (usually attacking or idle)
        if (Ability2.GetStatus() != BasicAbility.SkillStatus.CASTING) SetStatus(_lastStatusBeforeCasting);
    }
    protected override void UpdateDeath()
    {
        UpdateAllSkills();

        Debug.Log(gameObject.name + " died heroically.");
        Destroy(this.gameObject);
    }

    /**
     * All skills moest be updated every frame so they can perform the cooldown and other effects
     * @TODO: perhaps coroutines are beter suited?
     */
    private void UpdateAllSkills()
    {
        HearthStone.SkillUpdate();
        Ability1.SkillUpdate();
        Ability2.SkillUpdate();
    }

    public float GetCurrentMana()
    {
        return _curMana;
    }

    public float GetMaxMana()
    {
        return _maxMana;
    }

    public void SetHomeBase(BaseControl homeBase)
    {
        this._homeBase = homeBase;
        this.SetAlliance(homeBase.getAlliance());
    }
}
