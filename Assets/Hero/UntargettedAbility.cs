using UnityEngine;

public class UntargettedAbility : BasicAbility{

    [SerializeField]
    protected ParticleSystem _effect;

    protected MOBAUnit.UnitStatus _previousStatus;

    public bool Activate()
    {
        if (GetStatus() != SkillStatus.READY)
        {
            Debug.Log(_hero.name + " can't activate " + AbilityName + ": timer is " + GetCurrentCoolDownTime());
            return false; //can't activate (yet)
        }

        Debug.Log(_hero.name + " activated " + AbilityName);
        if (_effect) _effect.Play();
        _previousStatus = _hero.GetStatus();
        SetStatus(SkillStatus.CASTING);
        return true;
    }

    public void DeActivate()
    {
        SetStatus(SkillStatus.COOLDOWN);
        if (_effect) _effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Debug.Log(_hero.name + " deactivated " + AbilityName);
    }

    protected override void UpdateCoolDown()
    {
        base.UpdateCoolDown();
        //if hero still thinks we're casting, change their status:
        if (_hero.GetStatus() == MOBAUnit.UnitStatus.ABILITY1) _hero.SetStatus(_previousStatus);
        if (_effect) _effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

}
