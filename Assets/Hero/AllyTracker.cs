using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AllyTracker : MonoBehaviour {

    private MOBAUnit.Alliance _alliance;
    private HashSet<MOBAUnit> _alliesInSight;

    private void Start()
    {
        _alliesInSight = new HashSet<MOBAUnit>();
    }
    public int GetNrAlliesInSight()
    {
        return this._alliesInSight.Count;
    }

    public IEnumerable<MOBAUnit> GetAlliesInSight()
    {
        _alliesInSight.RemoveWhere(isNull);
        return this._alliesInSight;
    }
    private static bool isNull(MOBAUnit u)
    {
        return u == null;
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
        if (unit != null && unit.GetAlliance() == _alliance)
        {
            _alliesInSight.Add(unit);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log(gameObject.name + " lost track of " + other.gameObject.name);
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null)
        {
            _alliesInSight.Remove(unit);
        }
    }
}
