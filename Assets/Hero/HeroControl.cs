using UnityEngine;

public class HeroControl : MOBAUnit {

    //FIELDS

    //references to abilities
    [SerializeField]
    private HearthStoneAbility _hearthStone;
    [SerializeField]
    private UntargettedAbility _ability1;
    [SerializeField]
    private FireBallAbility _ability2;

    [SerializeField]
    private float walkSpeed = 1.5f;
    [SerializeField]
    private float runSpeed = 3f;
    [SerializeField]
    private float closeEnoughForMove = 0.4f;
    [SerializeField]
    private float closeEnoughForAttack = 1f;

    [SerializeField]
    private float _maxMana;

    //privates
    private float _curMana;
    private Vector3 _moveDestination;
    private MOBAUnit _attackTarget;
    private float _attackDelay = 1f;
    private float _attackedLastTime;
    private BaseControl _homeBase;

	// Use this for initialization
	override protected void Start () {
        base.Start();
        _curMana = 0;
        _attackedLastTime = 0;

	}

	/**
     * This public method is used by the GUI, the AI or the network to make the hero do something
     */
    public void MoveTo(Vector3 target, bool running = false)
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
    }

    /**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
     */
    public void ActivateHearthStone()
    {
        //first check legal status? Ability will check correct timing
        if (this._hearthStone.Activate())
        {
            //hero status should be changed by ability
            SetStatus(UnitStatus.HEARTHSTONE);
        }
    }
    public float GetHearthStoneCoolDown()
    {
        return _hearthStone.GetCurrentCoolDownTime();
    }

    /**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
     */
    public void ActivateAbility1()
    {
        //first check legal status? Ability will check correct timing
        if (_ability1.Activate())
        {
            //hero status should be changed by ability
            SetStatus(UnitStatus.ABILITY1);
        }
    }
    public float GetAbility1CoolDown()
    {
        return _ability1.GetCurrentCoolDownTime();
    }

    /**
     * Tells the GUI what the "valid" targets are for ability2
     */
    public int GetAbility2Mask()
    {
        return _ability2.GetValidTargetMask();
    }
    public float GetAbility2CoolDown()
    {
        return _ability2.GetCurrentCoolDownTime();
    }

    /**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
     */
    public void ActivateAbility2(GameObject target)
    {
        //first check legal status? Ability will check correct timing
        Rotate2DTo(target.transform.position);
        if (_ability2.Activate(target))
        {
            //hero status should be changed by ability
            SetStatus(UnitStatus.ABILITY2);
        }
    }

    protected override void UpdateIdle()
    {
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
        Rotate2DTo(_moveDestination);
        float distSqr = (transform.position - _moveDestination).sqrMagnitude;
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

    private void Rotate2DTo(Vector3 dest)
    {
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
        Rotate2DTo(_attackTarget.transform.position);
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
        if (Time.time - _attackedLastTime > _attackDelay)
        {
            _attackedLastTime = Time.time;
        }
    }

    protected override void UpdateHearthStone()
    {
    }

    protected override void UpdateAbility1()
    {
    }
    protected override void UpdateAbility2()
    {
    }
    protected override void UpdateDeath()
    {
    }

    protected void DeathAnimationDone()
    {
        Destroy(this.gameObject, 5f);
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
        _hearthStone.SetHearthStone(homeBase.SpawnPoint);
        this.SetAlliance(homeBase.getAlliance());
    }

    //pass on this animation event
    private void CastHearthStoneSpell()
    {
        _hearthStone.CastHearthStoneSpell();
    }

    //pass on this animation event
    private void EndHearthStone()
    {
        _hearthStone.EndHearthStone();
    }

    private void ThrowFireBall()
    {
        _ability2.ThrowFireBall();
    }
}
