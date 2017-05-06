using System.Collections.Generic;
using UnityEngine;

public class MinionAI : MOBAUnit {

    //FIELDS
    //References
    private Animator _animator;
    private CharacterController _charCtrl;

    //editor fields
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    MOBAUnit.DamageType damageType;
    [SerializeField]
    private float damagePerAttack;

    //private fields
    private Transform[] _waypoints;
    private int curWaypoint = -1;

    //keep track of the enemy we're currently attacking:
    private HashSet<MOBAUnit> enemiesInSight;
    private MOBAUnit _targetEnemy;
    private float _targetEnemyDistance;

    private const float CloseToWaypoint = 0.4f;
    private const float CloseToAttack = 1f;

    override public void Start () {
        base.Start();
        Debug.Log("MinionAI.Start");
        _animator = GetComponent<Animator>();
        enemiesInSight = new HashSet<MOBAUnit>();

	}

    void Update() {

        if (GetHealth() < 0)
        {
            Debug.Log("Minion has died");
            Destroy(this.gameObject);
        }

        if (enemiesInSight.Count > 0)
        {
            Attack();
        }
        else
        {
            MoveToWaypoint();
        }
        _animator.SetInteger("nrEnemies", enemiesInSight.Count);
    }

    private void Attack()
    {
        if (_targetEnemy == null)
        {
            ChooseClosestEnemy();
        }

        _targetEnemyDistance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        _animator.SetFloat("ClosestEnemy", _targetEnemyDistance);
        if (_targetEnemyDistance > CloseToAttack)
        {
            MoveAttack();
        }
        else
        {
            //do nothing; attack method will be called by animation when sword is thrust forward
            //MeleeAttack();
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
            _targetEnemy.ReceiveDamage(this.damageType, this.damagePerAttack);
        }
    }

    private void Rotate2DTo(Transform target)
    {
        Vector3 dest = target.position;
        dest.y = transform.position.y; //set destination at same height to avoid pitch change
        transform.LookAt(dest);
    }
    private void ChooseClosestEnemy()
    {
        _targetEnemyDistance = float.MaxValue;
        foreach (MOBAUnit enemy in enemiesInSight)
        {
            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d < _targetEnemyDistance)
            {
                _targetEnemy = enemy;
                _targetEnemyDistance = d;
            }
        }
        Debug.Log("Attacking " + _targetEnemy.name + " at distance " + _targetEnemyDistance);

    }

    private void MoveAttack()
    {
        _animator.SetFloat("Speed", 2.0f);
        Rotate2DTo(_targetEnemy.transform);
        transform.position += transform.forward * Time.deltaTime * runSpeed;
    }

    private void MoveToWaypoint()
    {
        if (curWaypoint < 0 || curWaypoint >= _waypoints.Length) return;

        float distanceToWaypoint = Vector3.Distance(transform.position, _waypoints[curWaypoint].position);
        if (distanceToWaypoint < CloseToWaypoint)
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
            _animator.SetFloat("Speed", 1f);
        }
        else
        {
            //no more waypoints, and apparently no enemies to attack -> idle
            _animator.SetFloat("Speed", 0f);
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

    private void OnTriggerEnter(Collider other)
    {
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null && unit.GetAlignment() != this.GetAlignment())
        {
            enemiesInSight.Add(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null)
        {
            enemiesInSight.Remove(unit);
            if (unit == _targetEnemy)
            {
                //out of range; no longer attacking this one
                //@TODO: setting targetEnemy to null may interact with the MeleeAttack() method
                _targetEnemy = null;
            }
        }
    }
}
