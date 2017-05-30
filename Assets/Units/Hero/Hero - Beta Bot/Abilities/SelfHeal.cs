using UnityEngine;

public class SelfHeal : UntargettedAbility
{

    [SerializeField]
    private float _healInterval = 2;
    [SerializeField]
    private float _healAmount = 5;

    private float _lastHealTime;

    protected override void UpdateEffect()
    {
        base.UpdateEffect();
        if (_lastHealTime + _healInterval < Time.time)
        {
            if (_hero.GetCurrentHealth() < _hero.GetMaxHealth())
            {
                _hero.AwardPoints((int)_healAmount);
                _hero.ReceiveDamage(MOBAUnit.DamageType.MAGIC, -_healAmount);
            }
            _lastHealTime = Time.time;
        }
    }
}
