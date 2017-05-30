using UnityEngine;

public class TargettedAbility: BasicAbility
{
    [SerializeField]
    private ParticleSystem effect;

    protected GameObject _target;
    protected Vector3 _point;

    virtual public int GetValidTargetMask()
    {
        //this ability targets units = layer 9
        return 1 << 9;
    }

    virtual public bool Activate(GameObject target, Vector3 point)
    {
        if (GetStatus() != SkillStatus.READY)
        {
            //@TODO: fizzle existing?
            Debug.Log(_hero.name + " can't activate " + AbilityName + ": timer is " + this.GetCurrentCoolDownTime());
            return false; //can't activate (yet)
        }

        Debug.Log(_hero.name + " activated " + AbilityName + " on " + target.name);
        SetStatus(SkillStatus.CASTING);
        _target = target;
        _point = point;
        if (effect) effect.Play(true);
        return true;
    }

    override public void DeActivate()
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
        if (effect && GetStatus() != SkillStatus.EFFECT) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

}