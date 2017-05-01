using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargettedSkill
{
    [SerializeField]
    private ParticleSystem effect;
    [SerializeField]
    private float CastDuration = 1f;
    [SerializeField]
    private float EffectDuration = 2f;
    [SerializeField]
    private float CoolDownTime = 2f;

    private enum SkillStatus {READY, CASTING, EFFECT, COOLDOWN }
    private SkillStatus status = SkillStatus.READY;
    private float lastStatusChange;

    private GameObject _target;

    public int GetValidTargetMask()
    {
        //this ability targets enemies = layer 9
        return 1 << 9;
    }

    public bool Activate(GameObject target)
    {
        if (status != SkillStatus.READY)
        {
            //@TBD: fizzle existing?
            Debug.Log("Hero can't activate ability 2: timer is " + this.GetCurrentCoolDownTime());
            return false; //can't activate (yet)
        }

        Debug.Log("Hero activated special ability 2 on " + target.name);
        status = SkillStatus.CASTING;
        _target = target;
        if (effect) effect.Play(true);
        this.lastStatusChange = Time.time;
        return true;
    }

    public void DeActivate()
    {
        //if (status) return;

        status = SkillStatus.COOLDOWN;
        _target = null;
        if (effect) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Debug.Log("Hero deactivated special ability 2");
    }

    /**
     * should be called every frame, to allow for continuous or delayed effects and animations
     */
    public void SkillUpdate()
    {
        switch (status)
        {
            case SkillStatus.COOLDOWN:
                UpdateCoolDown();
                break;
            case SkillStatus.CASTING:
                UpdateCasting();
                break;
            case SkillStatus.EFFECT:
                UpdateEffect();
                break;
        }
    }

    private void UpdateEffect()
    {

        if (lastStatusChange + EffectDuration < Time.time)
        {
            Debug.Log("Hero Ability2 finished -- " + lastStatusChange + " + " + EffectDuration + " < " + Time.time);
            status = SkillStatus.COOLDOWN;
            lastStatusChange = Time.time;
        }
    }

    private void UpdateCasting()
    {
        if (lastStatusChange + CastDuration < Time.time)
        {
            Debug.Log("Hero done casting Ability2 -- " + lastStatusChange + " + " + CastDuration + " < " + Time.time);
            status = SkillStatus.EFFECT;
            lastStatusChange = Time.time;
        }
    }

    private void UpdateCoolDown()
    {
        if (lastStatusChange + CoolDownTime < Time.time)
        {
            Debug.Log("Hero Ability2 is ready -- " + lastStatusChange + " + " + CoolDownTime + " < " + Time.time);
            status = SkillStatus.READY;
            lastStatusChange = Time.time;
        }
    }

    public bool IsActive()
    {
        return status == SkillStatus.CASTING || status == SkillStatus.EFFECT;
    }

    public bool IsAvailable()
    {
        return status == SkillStatus.READY;
    }

    public float GetCurrentCoolDownTime()
    {
        if (status == SkillStatus.COOLDOWN)
            return (this.lastStatusChange + this.CoolDownTime) - Time.time;
        else
            return 0;
    }

}