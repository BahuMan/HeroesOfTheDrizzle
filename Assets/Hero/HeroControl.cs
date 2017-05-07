using System;
using System.Collections;
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
    private UntargettedSkill Ability1;

    [SerializeField]
    private TargettedSkill Ability2;

	// Use this for initialization
	override protected void Start () {
        base.Start();
        _attackedLastTime = 0;
	}

	/**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
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
    public void ActivateAbility1()
    {
        //first check legal status? Ability will check correct timing
        if (this.Ability1.Activate())
        {
            SetStatus(UnitStatus.ABILITY1);
        }
    }

    /**
     * Tells the GUI what the "valid" targets are for ability2
     */
    public int GetAbility2Mask()
    {
        return Ability2.GetValidTargetMask();
    }

    /**
     * This public method is used buy the GUI, the AI or the network to make the hero do something
     */
    public void ActivateAbility2(GameObject target)
    {
        //first check legal status? Ability will check correct timing
        if (this.Ability2.Activate(target))
        {
            SetStatus(UnitStatus.ABILITY2);
        }
    }

    protected override void UpdateIdle()
    {
        //nuth'n
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
        if (Time.time - _attackedLastTime > _attackDelay)
        {
            _attackedLastTime = Time.time;
        }
    }

    protected override void UpdateAbility1()
    {
        Ability1.SkillUpdate();
    }
    protected override void UpdateAbility2()
    {
        Ability2.SkillUpdate();
    }
    protected override void UpdateDeath()
    {
        Debug.Log(gameObject.name + " died heroically.");
        Destroy(this.gameObject);
    }

}
