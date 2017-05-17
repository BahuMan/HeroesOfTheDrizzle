﻿using UnityEngine;

public class TargettedAbility: BasicAbility
{
    [SerializeField]
    private ParticleSystem effect;

    private GameObject _target;

    virtual public int GetValidTargetMask()
    {
        //this ability targets enemies = layer 9
        return 1 << 9;
    }

    virtual public bool Activate(GameObject target)
    {
        if (GetStatus() != SkillStatus.READY)
        {
            //@TBD: fizzle existing?
            Debug.Log(_hero.name + " can't activate " + AbilityName + ": timer is " + this.GetCurrentCoolDownTime());
            return false; //can't activate (yet)
        }

        Debug.Log(_hero.name + " activated " + AbilityName + " on " + target.name);
        SetStatus(SkillStatus.CASTING);
        _target = target;
        if (effect) effect.Play(true);
        return true;
    }

    public void DeActivate()
    {
        //if (status) return;

        SetStatus(SkillStatus.COOLDOWN);
        _target = null;
        if (effect) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Debug.Log(_hero.name + "deactivated " + AbilityName);
    }

    override protected void UpdateEffect()
    {
        base.UpdateEffect();
        if (GetStatus() != SkillStatus.EFFECT) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

}