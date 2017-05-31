using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyTracker : MonoBehaviour {

    [SerializeField]
    private MOBAUnit.Alliance _alliance;
    private HashSet<MOBAUnit> _enemiesInSight = new HashSet<MOBAUnit>();

    public bool IsInSight(MOBAUnit u)
    {
        return this._enemiesInSight.Contains(u);
    }

    public int GetNrEnemiesInSight()
    {
        _enemiesInSight.RemoveWhere(isNullOrDead);
        return this._enemiesInSight.Count;
    }

    public IEnumerable<MOBAUnit> GetEnemiesInSight()
    {
        _enemiesInSight.RemoveWhere(isNullOrDead);
        return this._enemiesInSight;
    }
    private static bool isNullOrDead(MOBAUnit u)
    {
        return (u == null || u.GetStatus() == MOBAUnit.UnitStatus.DEATH);
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
        //Debug.Log(gameObject.name + " lost track of " + other.gameObject.name);
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null)
        {
            _enemiesInSight.Remove(unit);
        }
    }
}
