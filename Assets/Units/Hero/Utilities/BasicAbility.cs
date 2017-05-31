using UnityEngine;

/**
 * Base class for any skill; continuous, targettable and non-targettable
 */
public class BasicAbility: MonoBehaviour
{

    public string AbilityName;

    [SerializeField]
    [Tooltip("How long it takes after activation, before the effect takes place. If you use event 'Ability2StartEffect' in the animation, you should set this high enough to become irrelevant")]
    protected float CastDuration = 1f;
    [SerializeField]
    [Tooltip("if your UpdateEffect() also calls base.UpdateEffect, the ability will automatically stop after this time")]
    protected float EffectDuration = .1f;
    [SerializeField]
    [Tooltip("How long you have to wait until you can cast this ability again. This is the countdown you see on the buttons")]
    protected float CoolDownTime = 5f;

    protected HeroControl _hero;
    public enum SkillStatus { READY, CASTING, EFFECT, COOLDOWN}
    private SkillStatus status = SkillStatus.READY;
    protected float lastStatusChange;
    private AudioSource _audioSource;

    virtual protected void Start()
    {
        _hero = GetComponentInParent<HeroControl>();
        _audioSource = GetComponent<AudioSource>();
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
     * you must call this method from the overriding method for the casting to end after a limited time.
     * Alternatively, your casting animation should contain an event "AbilityStartEffect", to be sent to HeroControl.
     * The HeroControl will forward the call to AbilityStartEffect() and thus change the status to "EFFECT"
     */
    virtual protected void UpdateCasting()
    {
        if (lastStatusChange + CastDuration < Time.time)
        {
            //instead of just changing the status to EFFECT, I'm calling this virtual method because
            //subclasses might override the method and do special stuff (like launching a fireball)
            //WARNING: if your CastDuration is shorter than the cast animation and the animation sends an event "Ability1StartEffect",
            //the following method is called twice. Make sure your CastDuration lasts long enough
            this.AbilityStartEffect();
        }
    }

    /**
     * you may call this method from the overriding method to have the effect expire after a limited time.
     */
    virtual protected void UpdateEffect()
    {
        if (lastStatusChange + EffectDuration < Time.time)
        {
            //Debug.Log(_hero.name + " finishing " + AbilityName);
            DeActivate();
        }
    }

    virtual public void DeActivate()
    {
        SetStatus(SkillStatus.COOLDOWN);
        //Debug.Log(_hero.name + " deactivated " + AbilityName);
    }


    /**
     * you must call this method from the overriding method for the cooldown to end after a limited time
     */
    virtual protected void UpdateCoolDown()
    {
        if (lastStatusChange + CoolDownTime < Time.time)
        {
            //Debug.Log(_hero.name + " ready to cast " + AbilityName);
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
        switch (status)
        {
            case SkillStatus.CASTING: return (this.lastStatusChange + this.CastDuration + this.EffectDuration + this.CoolDownTime) - Time.time;
            case SkillStatus.EFFECT: return (this.lastStatusChange + this.EffectDuration + this.CoolDownTime) - Time.time;
            case SkillStatus.COOLDOWN: return (this.lastStatusChange + this.CoolDownTime) - Time.time;
            default:
                return 0;
        }
    }

    /**
     * this method should be called during the animation frame where the casting of the spell
     * starts to take effect, typically a dramatic pose in the middle of the casting animation.
     * The animation can continue, but from now on the ability will be in effect.
     * This will not automatically change the ability on the hero (the hero is still "preoccupied" with this ability)
     */
    virtual public void AbilityStartEffect()
    {
        Debug.Log(_hero.name + " start effect " + AbilityName);
        if (_audioSource) _audioSource.Play();
        SetStatus(SkillStatus.EFFECT);
    }

    /**
     * this method should be called during the animation frame where the casting of the spell
     * and all the arm waving has finished. The actual effect may continue, but the hero is no longer
     * preoccupied with the gestures and can resume business as usual (idle, killing, fighting)
     */
    virtual public void AbilityCastingFinished()
    {
        _hero.SetStatus(MOBAUnit.UnitStatus.IDLE);
    }
}
