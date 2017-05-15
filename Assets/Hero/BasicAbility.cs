using UnityEngine;

/**
 * Base class for any skill; continuous, targettable and non-targettable
 */
[RequireComponent(typeof(HeroControl))]
public class BasicAbility: MonoBehaviour
{

    public string AbilityName;

    [SerializeField]
    protected float CastDuration = 1f;
    [SerializeField]
    protected float EffectDuration = .1f;
    [SerializeField]
    protected float CoolDownTime = 5f;

    protected HeroControl _hero;
    public enum SkillStatus { READY, CASTING, EFFECT, COOLDOWN}
    private SkillStatus status = SkillStatus.READY;
    protected float lastStatusChange;

    virtual protected void Start()
    {
        _hero = GetComponent<HeroControl>();
    }


    /**
     * Update can't be overridden by derived; you should override one of the
     * specific methods UpdateReady(), UpdateCasting(), UpdateEffect(), UpdateCoolDown()
     */
    public void Update()
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
     * you must call this method from the overriding method for the casting to end after a limited time
     */
    virtual protected void UpdateCasting()
    {
        if (lastStatusChange + CastDuration < Time.time)
        {
            Debug.Log(_hero.name + " done casting " + AbilityName);
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
            Debug.Log(_hero.name + " finished " + AbilityName);
            status = SkillStatus.COOLDOWN;
            lastStatusChange = Time.time;
        }
    }

    /**
     * you must call this method from the overriding method for the cooldown to end after a limited time
     */
    virtual protected void UpdateCoolDown()
    {
        if (lastStatusChange + CoolDownTime < Time.time)
        {
            Debug.Log(_hero.name + " ready to cast " + AbilityName);
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
