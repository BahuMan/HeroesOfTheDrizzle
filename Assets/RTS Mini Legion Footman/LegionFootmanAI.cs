using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionFootmanAI : MonoBehaviour {

    //FIELDS
    //References
    private Animator _animator;
    private CharacterController _charCtrl;

    //editor fields
    [SerializeField]
    private GameObject WaypointList;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float maxHealth;

    //private fields
    private List<Transform> _waypoints;
    private int curWaypoint = -1;
    private float curHealth;

    //keep track of the enemy we're currently attacking:
    private HashSet<GameObject> enemiesInSight;
    private GameObject _targetEnemy;
    private float _targetEnemyDistance;

    private const float CloseToWaypoint = 0.4f;
    private const float CloseToAttack = 1f;

    // Use this for initialization
    void Start () {
        _animator = GetComponent<Animator>();
        enemiesInSight = new HashSet<GameObject>();

        _waypoints = new List<Transform>();
        foreach (Transform child in WaypointList.transform)
        {
            _waypoints.Add(child);
        }
        curWaypoint = 0;
	}

    // Update is called once per frame
    void Update() {

        if (enemiesInSight.Count > 0)
        {
            Attack();
        }
        else
        {
            MoveWaypoint();
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
        //can I ask the animator's state and use that, instead of recalculating my own state?
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

    public void MeleeAttack()
    {
        //Debug.Log("Melee Attack");
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
        foreach (GameObject enemy in enemiesInSight)
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

    private void MoveWaypoint()
    {
        if (curWaypoint < 0 && curWaypoint >= _waypoints.Count) return;

        float distanceToWaypoint = Vector3.Distance(transform.position, _waypoints[curWaypoint].position);
        if (distanceToWaypoint < CloseToWaypoint)
        {
            ++curWaypoint;
        }

        if (curWaypoint < _waypoints.Count)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemiesInSight.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemiesInSight.Remove(other.gameObject);
            if (other.gameObject == _targetEnemy)
            {
                //out of range; no longer attacking this one
                _targetEnemy = null;
            }
        }
    }
}
