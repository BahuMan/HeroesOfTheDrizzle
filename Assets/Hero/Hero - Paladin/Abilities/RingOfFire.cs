using System.Collections;
using UnityEngine;

public class RingOfFire : UntargettedAbility {

    [SerializeField]
    private float _damage = 1f;
    [SerializeField]
    private float _timeBetweenDamage = 1f;
    [SerializeField]
    private float _sqrDistance = 4f;

    private ParticleSystem[] _fireSystems;
    private float _lastDamageTime = 0f;

    protected override void Start()
    {
        base.Start();
        _fireSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public override bool Activate()
    {
        if (base.Activate())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void DeActivate()
    {
        base.DeActivate();
        VisualizeFire(false);
    }

    private void VisualizeFire(bool show)
    {
        foreach (ParticleSystem ps in _fireSystems)
        {
            if (show) ps.Play();
            else ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    protected override void UpdateEffect()
    {
        base.UpdateEffect();
        if (_lastDamageTime + _timeBetweenDamage < Time.time)
        {
            VisualizeFire(true);
            burn();
            _lastDamageTime = Time.time;
        }

    }

    private void burn()
    {
        MOBAUnit.Alliance ourSide = _hero.GetAlliance();
        foreach (MOBAUnit unit in _hero.GetEnemiesInSight())
        {
            //if enemy and close enough ...
            if (unit.GetAlliance() != ourSide && (_hero.transform.position - unit.transform.position).sqrMagnitude < _sqrDistance)
            {
                Debug.Log(_hero.name + " dealing fire damage to " + unit.name);
                unit.ReceiveDamage(MOBAUnit.DamageType.FIRE, _damage);
            }
        }

    }
}
