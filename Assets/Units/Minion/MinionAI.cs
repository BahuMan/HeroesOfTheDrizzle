using System.Collections.Generic;
using UnityEngine;

public class MinionAI : MOBAUnit {

    //FIELDS
    //References
    private CharacterController _charCtrl;
    private EnemyTracker _enemyTracker;

    //editor fields
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float damagePerAttack;

    //private fields
    [SerializeField]
    private Transform[] _waypoints;
    private int curWaypoint = -1;

    //keep track of the enemy we're currently attacking:
    private MOBAUnit _targetEnemy;
    private float _targetEnemyDistance;

    //(square) distances at which we consider the goal "reached":
    private const float CloseEnoughToWaypoint = 0.2f;
    private const float CloseEnoughToAttack = 4f;

    //fall accelleration:
    private Vector3 curFallSpeed = Vector3.zero;

    override protected void Start () {
        base.Start();
        _charCtrl = GetComponent<CharacterController>();
        _enemyTracker = GetComponentInChildren<EnemyTracker>();
        //Debug.Log("MinionAI.Start");
        if (_waypoints != null && _waypoints.Length > 0) curWaypoint = 0;
	}

    override protected void UpdateIdle()
    {
        CommonUpdate();
    }

    override protected void UpdateWalking()
    {
        CommonUpdate();
    }

    override protected void UpdateRunning()
    {
        CommonUpdate();
    }

    override protected void UpdateAttacking()
    {

        if (_enemyTracker.GetNrEnemiesInSight() == 0)
        {
            SetStatus(UnitStatus.IDLE);
            return;
        }

        if (_targetEnemy == null)
        {
            _targetEnemy = _enemyTracker.ChooseClosestEnemy();
            //_targetEnemyDistance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        }

        _targetEnemyDistance = (_targetEnemy.transform.position - transform.position).sqrMagnitude;
        GetAnimatorComponent().SetFloat("ClosestEnemy", _targetEnemyDistance);
        if (_targetEnemyDistance > CloseEnoughToAttack)
        {
            SetStatus(UnitStatus.ATTACKRUNNING);
        }
        else
        {
            SetStatus(UnitStatus.ATTACKING);
            //actual damage will be dealt at appropriate frame in the attack animation
            //(with callback to MeleeAttack())
        }
    }

    override protected void UpdateAttackRunning()
    {

        if (_enemyTracker.GetNrEnemiesInSight() == 0)
        {
            SetStatus(UnitStatus.IDLE);
            return;
        }

        if (_targetEnemy == null)
        {
            _targetEnemy = _enemyTracker.ChooseClosestEnemy();
            //_targetEnemyDistance = (_targetEnemy.transform.position - transform.position).sqrMagnitude;
        }

        _targetEnemyDistance = (_targetEnemy.transform.position - transform.position).sqrMagnitude;
        GetAnimatorComponent().SetFloat("ClosestEnemy", _targetEnemyDistance);
        if (_targetEnemyDistance > CloseEnoughToAttack)
        {
            GetAnimatorComponent().SetFloat("Speed", 2.0f);
            Rotate2DTo(_targetEnemy.transform);
            //transform.position += transform.forward * Time.deltaTime * runSpeed;
            _charCtrl.Move(transform.forward * Time.deltaTime * runSpeed);
        }
        else
        {
            SetStatus(UnitStatus.ATTACKING);
        }

    }

    override protected void UpdateHearthStone() { Debug.LogError("Minions don't have HearthStone"); }
    override protected void UpdateAbility1() { Debug.LogError("Minions don't have any abilities"); }
    override protected void UpdateAbility2() { Debug.LogError("Minions don't have any abilities"); }
    override protected void UpdateDeath()
    {
        Debug.Log(this.gameObject.name + " died.");
        Destroy(this.gameObject);
    }


    private void CommonUpdate()
    {
        if (_enemyTracker.GetNrEnemiesInSight() > 0)
        {
            SetStatus(UnitStatus.ATTACKING);
        }
        else
        {
            SetStatus(UnitStatus.WALKING);
            MoveToWaypoint();
        }
    }

    /**
     * This method is called by the animation at the frame where the spear is thrust out the furthest
     */
    public void MeleeAttack()
    {
        //Debug.Log("Melee Attack");
        if (_targetEnemy != null)
        {
            _targetEnemy.ReceiveDamage(GetDamageType(), this.damagePerAttack);
        }
    }

    private void Rotate2DTo(Transform target)
    {
        Vector3 dest = target.position;
        dest.y = transform.position.y; //set destination at same height to avoid pitch change
        transform.LookAt(dest);
    }



    private void MoveToWaypoint()
    {
        if (curWaypoint < 0 || curWaypoint >= _waypoints.Length) return;

        float distanceToWaypoint = (_waypoints[curWaypoint].position - transform.position).sqrMagnitude;
        if (distanceToWaypoint < CloseEnoughToWaypoint)
        {
            ++curWaypoint;
        }

        if (curWaypoint < _waypoints.Length)
        {
            Vector3 direction = _waypoints[curWaypoint].position - transform.position;
            direction.y = 0f;
            direction = direction.normalized * walkSpeed;
            //Quaternion targetRotation = Quaternion.LookRotation(direction);
            //_charCtrl.transform.rotation = Quaternion.Slerp(_charCtrl.transform.rotation, targetRotation, rotationSpeed);
            transform.rotation = Quaternion.LookRotation(direction);
            if (_charCtrl.isGrounded)
            {
                curFallSpeed = Vector3.zero;
            }
            else
            {
                curFallSpeed += Physics.gravity * Time.deltaTime;
                direction += curFallSpeed;
            }
            _charCtrl.Move(direction  * Time.deltaTime);
            //transform.position += direction * walkSpeed * Time.deltaTime;
            GetAnimatorComponent().SetFloat("Speed", walkSpeed);
        }
        else
        {
            //no more waypoints, and apparently no enemies to attack -> idle
            SetStatus(UnitStatus.IDLE);
            GetAnimatorComponent().SetFloat("Speed", 0f);
        }
    }

    /*
     * Overwrite the current waypoints with a new set
     * the first waypoint is assumed to be the closest by, so the current waypoint index is reset to zero
     * @TODO all waypoints are assumed to be navigable in a straight line (no pathfinding between 2 waypoints)
     */
    public void setWaypoints(Transform[] points)
    {
        this._waypoints = points;
        this.curWaypoint = 0;
    }

}
