using UnityEngine;

[System.Serializable]
public class TargettedSkill: BasicSkill
{
    [SerializeField]
    private ParticleSystem effect;

    private GameObject _target;

    public TargettedSkill(): base("Targetted Ability2")
    {
    }

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
            Debug.Log("Hero can't activate " + _name + ": timer is " + this.GetCurrentCoolDownTime());
            return false; //can't activate (yet)
        }

        Debug.Log("Hero activated " + _name + " on " + target.name);
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
        Debug.Log("Hero deactivated special ability 2");
    }

    override protected void UpdateEffect()
    {
        base.UpdateEffect();
        if (GetStatus() == SkillStatus.COOLDOWN) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

}