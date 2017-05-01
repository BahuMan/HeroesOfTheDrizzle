using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UntargettedSkill : BasicSkill{

    [SerializeField]
    private ParticleSystem effect;

    public UntargettedSkill(): base("UntargettedSkill")
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
        if (effect) effect.Play();
        SetStatus(SkillStatus.CASTING);
        return true;
    }

    public void DeActivate()
    {
        SetStatus(SkillStatus.COOLDOWN);
        if (effect) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Debug.Log("Hero deactivated " + _name);
    }

    override protected void UpdateEffect()
    {
        base.UpdateEffect();
        if (GetStatus() == SkillStatus.COOLDOWN) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

}
