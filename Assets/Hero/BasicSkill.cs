using UnityEngine;

/**
 * Base class for any skill; continuous, targettable and non-targettable
 */
[System.Serializable]
public class BasicSkill
{
    [SerializeField]
    protected float CastDuration = 1f;
    [SerializeField]
    protected float EffectDuration = 2f;
    [SerializeField]
    protected float CoolDownTime = 2f;

    protected string _name;
    public enum SkillStatus { READY, CASTING, EFFECT, COOLDOWN}
    private SkillStatus status = SkillStatus.READY;
    protected float lastStatusChange;

    protected BasicSkill(string name)
    {
        _name = name;
    }

    public void SkillUpdate()
    {
        switch (status)
        {
            case SkillStatus.READY:
                UpdateReady();
                break;
            case SkillStatus.CASTING:
                UpdateCasting();
                break;
            case SkillStatus.EFFECT:
                UpdateEffect();
                break;
            case SkillStatus.COOLDOWN:
                UpdateCoolDown();
                break;
        }
    }

    virtual protected void UpdateReady()
    {
        //do nothing
        //continuous effects might want to override this method
    }

    /**
     * you must call this method from the overriding method to have a timed casting
     */
    virtual protected void UpdateCasting()
    {
        if (lastStatusChange + CastDuration < Time.time)
        {
            Debug.Log("Hero done casting " + _name);
            status = SkillStatus.EFFECT;
            lastStatusChange = Time.time;
        }
    }

    /**
     * you may call this method from the overriding method to have a timed effect
     */
    virtual protected void UpdateEffect()
    {
        if (lastStatusChange + EffectDuration < Time.time)
        {
            Debug.Log("Hero finished " + _name);
            status = SkillStatus.COOLDOWN;
            lastStatusChange = Time.time;
        }
    }
    virtual protected void UpdateCoolDown()
    {
        if (lastStatusChange + CoolDownTime < Time.time)
        {
            Debug.Log("Hero ready to cast " + _name);
            status = SkillStatus.READY;
            lastStatusChange = Time.time;
        }
    }

    public SkillStatus GetStatus()
    {
        return status;
    }

    public void SetStatus(SkillStatus newStatus)
    {
        if (status != newStatus) lastStatusChange = Time.time;
        status = newStatus;
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
