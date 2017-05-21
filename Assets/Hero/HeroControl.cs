using UnityEngine;

public class HeroControl : MOBAUnit {

    //FIELDS

    //references to abilities
    [SerializeField]
    private HearthStoneAbility _hearthStone;
    [SerializeField]
    private UntargettedAbility _ability1;
    [SerializeField]
    private TargettedAbility _ability2;

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
    [SerializeField]
    private int _points;
    [SerializeField]
    private HeroUpgrade[] _upgrades;

    //privates
    private float _curMana;
    private Vector3 _moveDestination;
    private MOBAUnit _attackTarget;
    private float _attackDelay = 1f;
    private float _attackedLastTime;
    private BaseControl _homeBase;

	override protected void Start () {
        base.Start();
        _curMana = 0;
        _points = 0;
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
        this.ChooseSides(homeBase.getAlliance());
    }

    /**
     * Called by the GUI, in case the player needs to choose an upgraded skill.
     * If this function returns a non-empty array, the choices will be presented to the player
     */
    public HeroUpgradeChoice[] GetUpgradeChoices()
    {
        if (_upgrades == null || _upgrades.Length == 0 || _upgrades[0].pointsRequired > _points) return null;
        else return _upgrades[0].options;
    }

    /**
     * When the player has made a choice out of the options returned in the above function GetUpgradeChoices(),
     * this set of upgrade choices is removed and the following set of options will be presented,
     * but only if the hero has gained enough points to qualify for the next upgrade
     */
    public void ChooseHeroUpgrade(int chosen)
    {
        Debug.Log("Hero upgrade chosen: " + _upgrades[0].options[chosen].Option);
        UpgradeToAbility(_upgrades[0].options[chosen].AbilityPrefab);
        //@TODO: remove the first item in the upgrades array, because that choice has now been made already
    }

    private void UpgradeToAbility(BasicAbility AbilityPrefab)
    {
        if (AbilityPrefab is TargettedAbility)
        {
            UpgradeTargettedAbility((TargettedAbility) AbilityPrefab);
        }
        else if (AbilityPrefab is UntargettedAbility)
        {
            UpgradeUntargettedAbility((UntargettedAbility) AbilityPrefab);
        }
        else
        {
            Debug.LogError("Not sure how to upgrade " + gameObject.name + " to ability " + AbilityPrefab.name);
        }
    }

    private void UpgradeTargettedAbility(TargettedAbility AbilityPrefab)
    {
        Destroy(this._ability2.gameObject);
        this._ability2 = Instantiate<TargettedAbility>(AbilityPrefab, this.transform);
    }

    private void UpgradeUntargettedAbility(UntargettedAbility AbilityPrefab)
    {
        Destroy(this._ability1.gameObject);
        this._ability1 = Instantiate<UntargettedAbility>(AbilityPrefab, this.transform);
    }

    /**
     * these methods can be events in the animation, to help time when the spell or attack is done,
     * and the effect can take place. This is the moment where the effect appears, the fireball is created, etc.
     */
    private void HearthStoneStartEffect()
    {
        _hearthStone.AbilityStartEffect();
    }

    /**
     * these methods can be events in the animation, to help time when the entire spellweaving is finished,
     * and the hero is no longer preoccupied with the spell. The effect of the spell might still last,
     * but the hero can change status to whatever is appropriate
     */
    private void HearthStoneCastingFinished()
    {
        _hearthStone.AbilityCastingFinished();
    }

    /**
     * these methods can be events in the animation, to help time when the spell or attack is done,
     * and the effect can take place. This is the moment where the effect appears, the fireball is created, etc.
     */
    private void Ability2StartEffect()
    {
        _ability2.AbilityStartEffect();
    }

    /**
     * these methods can be events in the animation, to help time when the entire spellweaving is finished,
     * and the hero is no longer preoccupied with the spell. The effect of the spell might still last,
     * but the hero can change status to whatever is appropriate
     */
    private void Ability2CastingFinished()
    {
        _ability2.AbilityCastingFinished();
    }

    /**
     * these methods can be events in the animation, to help time when the spell or attack is done,
     * and the effect can take place. This is the moment where the effect appears, the fireball is created, etc.
     */
    private void Ability1StartEffect()
    {
        _ability1.AbilityStartEffect();
    }

    /**
     * these methods can be events in the animation, to help time when the entire spellweaving is finished,
     * and the hero is no longer preoccupied with the spell. The effect of the spell might still last,
     * but the hero can change status to whatever is appropriate
     */
    private void Ability1CastingFinished()
    {
        _ability1.AbilityCastingFinished();
    }

}
