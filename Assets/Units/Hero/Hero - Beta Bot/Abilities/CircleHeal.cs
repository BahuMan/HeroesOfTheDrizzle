using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CircleHeal : UntargettedAbility
{
    [SerializeField]
    private float _healInterval = 2;
    [SerializeField]
    private float _healAmount = 5;
    [SerializeField]
    private float _healDistance = 5;

    private float _lastHealTime;
    private List<MOBAUnit> _friendlies = new List<MOBAUnit>();

    protected override void UpdateEffect()
    {
        base.UpdateEffect();
        if (_lastHealTime + _healInterval < Time.time)
        {
            HealAllies();
            _lastHealTime = Time.time;
        }
    }

    private void HealAllies()
    {
        foreach (MOBAUnit unit in _friendlies)
        {
            if (unit.GetCurrentHealth() < unit.GetMaxHealth())
            {
                _hero.AwardPoints((int) _healAmount);
                unit.ReceiveDamage(MOBAUnit.DamageType.MAGIC, -_healAmount);
            }
        }
    }

    //@TODO separate this into a separate script, trackFriendlies (and TrackEnemies)
    public void OnTriggerEnter(Collider other)
    {
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null && unit.GetAlliance() == _hero.GetAlliance())
        {
            _friendlies.Add(unit);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log(gameObject.name + " lost track of " + other.gameObject.name);
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null)
        {
            _friendlies.Remove(unit);
        }
    }
}
