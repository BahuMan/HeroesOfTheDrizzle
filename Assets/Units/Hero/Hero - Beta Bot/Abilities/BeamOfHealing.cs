using System.Collections;
using UnityEngine;

public class BeamOfHealing: TargettedAbility
{
    [SerializeField]
    private float _healInterval = 2;
    [SerializeField]
    private float _healAmount = 5;
    [SerializeField]
    private LineRenderer _beam;
    [SerializeField]
    private Vector3 _beamOffset = new Vector3(0, 1, 0);

    private float _lastHealTime;
    private MOBAUnit _targetUnit;

    override public bool Activate(GameObject target, Vector3 point)
    {
        if (base.Activate(target, point))
        {
            _targetUnit = target.GetComponent<MOBAUnit>();
            if (_targetUnit == null) return false;
            else return true;
        }
        else return false;
    }

    override public void AbilityStartEffect()
    {
        base.AbilityStartEffect();
        _beam.gameObject.SetActive(true);
    }

    protected override void UpdateEffect()
    {
        base.UpdateEffect();

        //if target died while healing, move to cooldown:
        if (_targetUnit == null)
        {
            SetStatus(SkillStatus.COOLDOWN);
        }

        //Set the position of the healing beam correctly:
        _beam.SetPosition(0, this.transform.position + _beamOffset);
        _beam.SetPosition(1, _targetUnit.transform.position + _beamOffset);

        //regularly top off healht:
        if (_lastHealTime + _healInterval < Time.time)
        {
            HealTarget();
            _lastHealTime = Time.time;
        }
    }

    private void HealTarget()
    {
        //if unit died, stop healing:
        if (!_targetUnit) DeActivate();

        //else, add some health, but no more than max health:
        float healthDifference = Mathf.Max(_targetUnit.GetMaxHealth() - _targetUnit.GetCurrentHealth(), _healAmount);
        _hero.AwardPoints((int)healthDifference);
        _targetUnit.ReceiveDamage(MOBAUnit.DamageType.MAGIC, -healthDifference);
    }

    public override void DeActivate()
    {
        base.DeActivate();
        _beam.gameObject.SetActive(false);
    }
}

