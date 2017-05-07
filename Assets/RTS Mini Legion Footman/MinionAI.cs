using System.Collections.Generic;
using UnityEngine;

public class MinionAI : MOBAUnit {

    //FIELDS
    //References
    private CharacterController _charCtrl;

    //editor fields
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float damagePerAttack;

    //private fields
    private Transform[] _waypoints;
    private int curWaypoint = -1;

    //keep track of the enemy we're currently attacking:
    private MOBAUnit _targetEnemy;
    private float _targetEnemyDistance;

    private const float CloseEnoughToWaypoint = 0.4f;
    private const float CloseEnoughToAttack = 1f;

    override protected void Start () {
        base.Start();
        Debug.Log("MinionAI.Start");
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

        if (GetHealth() < 0)
        {
            SetStatus(UnitStatus.DEATH);
            return;
        }

        if (GetNrEnemiesInSight() == 0)
        {
            SetStatus(UnitStatus.IDLE);
            return;
        }

        if (_targetEnemy == null)
        {
            _targetEnemy = ChooseClosestEnemy();
            _targetEnemyDistance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        }

        _targetEnemyDistance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        GetAnimatorComponent().SetFloat("ClosestEnemy", _targetEnemyDistance);
        if (_targetEnemyDistance > CloseEnoughToAttack)
        {
            SetStatus(UnitStatus.ATTACKRUNNING);
        }
        else
        {
            //do nothing; attack method will be called by animation when sword is thrust forward
            //MeleeAttack();
        }
    }

    override protected void UpdateAttackRunning()
    {

        if (GetHealth() < 0)
        {
            SetStatus(UnitStatus.DEATH);
            return;
        }

        if (GetNrEnemiesInSight() == 0)
        {
            SetStatus(UnitStatus.IDLE);
            return;
        }

        if (_targetEnemy == null)
        {
            _targetEnemy = ChooseClosestEnemy();
            _targetEnemyDistance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        }

        _targetEnemyDistance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        GetAnimatorComponent().SetFloat("ClosestEnemy", _targetEnemyDistance);
        if (_targetEnemyDistance > CloseEnoughToAttack)
        {
            GetAnimatorComponent().SetFloat("Speed", 2.0f);
            Rotate2DTo(_targetEnemy.transform);
            transform.position += transform.forward * Time.deltaTime * runSpeed;
        }
        else
        {
            SetStatus(UnitStatus.ATTACKING);
        }

    }

    override protected void UpdateAbility1() { Debug.LogError("Minions don't have any abilities"); }
    override protected void UpdateAbility2() { Debug.LogError("Minions don't have any abilities"); }
    override protected void UpdateDeath()
    {
        Debug.Log(this.gameObject.name + " died.");
        Destroy(this.gameObject);
    }


    private void CommonUpdate() {

        if (GetHealth() < 0)
        {
            Debug.Log("Minion has died");
            SetStatus(UnitStatus.DEATH);
        }

        if (GetNrEnemiesInSight() > 0)
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
        //@TODO: what to do when enemy has died by another friendly and I'm still trying to damage it? See OnTriggerExit()
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

        float distanceToWaypoint = Vector3.Distance(transform.position, _waypoints[curWaypoint].position);
        if (distanceToWaypoint < CloseEnoughToWaypoint)
        {
            ++curWaypoint;
        }

        if (curWaypoint < _waypoints.Length)
        {
            Vector3 direction = _waypoints[curWaypoint].position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            //Quaternion targetRotation = Quaternion.LookRotation(direction);
            //_charCtrl.transform.rotation = Quaternion.Slerp(_charCtrl.transform.rotation, targetRotation, rotationSpeed);
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * walkSpeed * Time.deltaTime;
            GetAnimatorComponent().SetFloat("Speed", 1f);
        }
        else
        {
            //no more waypoints, and apparently no enemies to attack -> idle
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
