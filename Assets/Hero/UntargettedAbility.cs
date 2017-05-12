using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UntargettedAbility : BasicAbility{

    [SerializeField]
    protected ParticleSystem _effect;

    public UntargettedAbility(HeroControl owner): base(owner, "UntargettedAbility")
    {

    }
    public UntargettedAbility(HeroControl owner, string name): base(owner, name)
    {

    }

    public bool Activate()
    {
        if (GetStatus() != SkillStatus.READY)
        {
            Debug.Log("Hero can't activate " + _name + ": timer is " + GetCurrentCoolDownTime());
            return false; //can't activate (yet)
        }

        Debug.Log("Hero activated " + _name);
        if (_effect) _effect.Play();
        SetStatus(SkillStatus.CASTING);
        return true;
    }

    public void DeActivate()
    {
        SetStatus(SkillStatus.COOLDOWN);
        if (_effect) _effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Debug.Log("Hero deactivated " + _name);
    }

    protected override void UpdateCoolDown()
    {
        base.UpdateCoolDown();
        _effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

}
