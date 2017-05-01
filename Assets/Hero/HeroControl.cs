using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeroControl : MonoBehaviour {

    //FIELDS
    //References
    Animator _animator;

    //private fields
    public enum HeroStatus {IDLE, WALKING, RUNNING, ATTACKRUNNING, ATTACKING, ABILITY1, ABILITY2}
    [SerializeField]
    private HeroStatus status = HeroStatus.IDLE;
    [SerializeField]
    private float walkSpeed = 1.5f;
    private Transform _moveDestination;
    private GameObject _attackTarget;
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
	void Start () {
        _animator = GetComponent<Animator>();
        _attackedLastTime = 0;
	}
	
	void Update () {
        switch (status) {
            case HeroStatus.IDLE:
                break;
            case HeroStatus.WALKING:
            case HeroStatus.RUNNING:
                UpdateMove();
                break;
            case HeroStatus.ATTACKING:
                UpdateAttack();
                break;
            case HeroStatus.ATTACKRUNNING:
                UpdateAttackMove();
                break;
            case HeroStatus.ABILITY1:
                Ability1.SkillUpdate();
                break;
            case HeroStatus.ABILITY2:
                Ability2.SkillUpdate();
                break;
            default:
                Debug.LogError("unknown state " + status.ToString());
                break;
        }

        _animator.SetInteger("HeroStatus", (int)status);
    }

    public void MoveTo(Transform target, bool running = false)
    {
        //first check illegal state transitions (are there any?)

        //then change status:
        _moveDestination = target;
        if (running)
        {
            status = HeroStatus.RUNNING;
        }
        else
        {
            status = HeroStatus.WALKING;
        }
        Debug.Log("I'm moving to " + target.name);
    }

    public void ActivateAbility1()
    {
        //first check legal status? Ability will check correct timing
        if (this.Ability1.Activate())
        {
            status = HeroStatus.ABILITY1;
        }
    }

    public int GetAbility2Mask()
    {
        return Ability2.GetValidTargetMask();
    }

    public void ActivateAbility2(GameObject target)
    {
        //first check legal status? Ability will check correct timing
        if (this.Ability2.Activate(target))
        {
            status = HeroStatus.ABILITY2;
        }
    }

    /**
     * Possible transitions:
     * -> IDLE (when destination has been reached)
     */
    void UpdateMove()
    {
        Rotate2DTo(_moveDestination);
        float distSqr = (transform.position - _moveDestination.position).sqrMagnitude;
        if (distSqr < closeEnoughForMove)
        {
            status = HeroStatus.IDLE;
            Debug.Log("I've reached my destination");
        }
        else
        {
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }
    }

    private void Rotate2DTo(Transform target)
    {
        Vector3 dest = target.position;
        dest.y = transform.position.y; //set destination at same height to avoid pitch change
        transform.LookAt(dest);
    }

    public void Attack(GameObject enemy)
    {
        //if necessary, check for illegal state transitions

        _attackTarget = enemy;
        if ((enemy.transform.position - transform.position).sqrMagnitude > closeEnoughForAttack)
        {
            Debug.Log("setting hero status to " + (int)HeroStatus.ATTACKRUNNING);
            status = HeroStatus.ATTACKRUNNING;
            UpdateAttackMove();
        }
        else
        {
            status = HeroStatus.ATTACKING;
            UpdateAttack();
        }
    }

    private void UpdateAttackMove()
    {
        Rotate2DTo(_attackTarget.transform);
        float distSqr = (transform.position - _attackTarget.transform.position).sqrMagnitude;
        if (distSqr < closeEnoughForAttack)
        {
            status = HeroStatus.ATTACKING;
            Debug.Log("I've reached my enemy");
            UpdateAttack();
        }
        else
        {
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }
    }

    private void UpdateAttack()
    {
        if (Time.time - _attackedLastTime > _attackDelay)
        {
            Debug.Log("Hero Attack");
            _attackedLastTime = Time.time;
        }
    }
}
