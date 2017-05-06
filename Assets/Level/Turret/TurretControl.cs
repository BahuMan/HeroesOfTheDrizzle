using System.Collections.Generic;
using UnityEngine;

public class TurretControl : MOBAUnit {

    [SerializeField]
    MOBAUnit.DamageType damageType;
    [SerializeField]
    private float damagePerAttack;
    [SerializeField]
    private float timeBetweenAttacks;

    private float lastAttackTime;
    private List<MOBAUnit> enemiesInSight;
    private MOBAUnit _targetEnemy;

    override public void Start () {
        base.Start();
        enemiesInSight = new List<MOBAUnit>();
	}
	
	void Update () {
        
        //do nothing if dead
        if (GetHealth() < 0) return;

        //pick an enemy
        if (_targetEnemy == null && enemiesInSight.Count > 0)
        {
            findEnemy();
        }

        //do nothing if nobody to be attacked
        if (_targetEnemy == null) return;

        //attack after delay
        if (lastAttackTime + timeBetweenAttacks < Time.time) {
            Fire();
        }
	}

    private void Fire()
    {
        Debug.Log(gameObject.name + " firing at " + _targetEnemy.name);
        _targetEnemy.ReceiveDamage(this.damageType, this.damagePerAttack);
        lastAttackTime = Time.time;
    }

    private void findEnemy()
    {
        _targetEnemy = enemiesInSight[Random.Range(0, enemiesInSight.Count)];
    }

    private void OnTriggerEnter(Collider other)
    {
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null && unit.GetAlignment() != this.GetAlignment())
        {
            enemiesInSight.Add(unit);
            if (enemiesInSight.Count == 1)
            {
                _targetEnemy = unit;
            }
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
