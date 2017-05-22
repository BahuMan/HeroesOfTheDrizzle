using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyTracker : MonoBehaviour {


    private MOBAUnit.Alliance _alliance;
    private HashSet<MOBAUnit> _enemiesInSight;

    private void Start()
    {
        _enemiesInSight = new HashSet<MOBAUnit>();
    }
    public int GetNrEnemiesInSight()
    {
        _enemiesInSight.RemoveWhere(isNull);
        return this._enemiesInSight.Count;
    }

    public IEnumerable<MOBAUnit> GetEnemiesInSight()
    {
        _enemiesInSight.RemoveWhere(isNull);
        return this._enemiesInSight;
    }
    private static bool isNull(MOBAUnit u)
    {
        return u == null;
    }

    public MOBAUnit ChooseClosestEnemy()
    {
        MOBAUnit closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (MOBAUnit enemy in GetEnemiesInSight())
        {
            //nullpointer check:
            //if (!enemy) continue;

            float d = (transform.position - enemy.transform.position).sqrMagnitude;
            if (d < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = d;
            }
        }
        return closestEnemy;
    }


    /**
     * This method is not called directly, but sent as a message
     * to all children of a hero GameObject.
     */
    private void SetAlliance(MOBAUnit.Alliance alliance)
    {
        _alliance = alliance;
    }

    public void OnTriggerEnter(Collider other)
    {
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null && unit.GetAlliance() != _alliance)
        {
            _enemiesInSight.Add(unit);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log(gameObject.name + " lost track of " + other.gameObject.name);
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null)
        {
            _enemiesInSight.Remove(unit);
        }
    }
}
