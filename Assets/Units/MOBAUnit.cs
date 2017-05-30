using System.Collections.Generic;
using UnityEngine;

/**
 * Base class for everything that can be attacked (buildings, units, minions, heroes).
 * Requires an Animator an will pass the following parameters to the animator:
 *   - UnitStatus
 *   - NrEnemies
 * Requires a trigger collider to detect enemies within its radius.
 */
[RequireComponent(typeof(Animator))]
public abstract class MOBAUnit : MonoBehaviour
{

    public enum UnitStatus { IDLE, WALKING, RUNNING, ATTACKRUNNING, ATTACKING, HEARTHSTONE, ABILITY1, ABILITY2, DEATH }
    public enum DamageType { MELEE, FIRE, PIERCE, MAGIC, CRUSH }
    public enum Alliance { NEUTRAL, BLUE, RED }
    [SerializeField]
    private UnitStatus _status;
    [SerializeField]
    private DamageType _damage;
    [SerializeField]
    private Alliance _camp;

    [SerializeField]
    private float _maxHealth;
    [SerializeField]
    private float _curHealth;

    //references
    private Animator _animator;

    virtual protected void Start()
    {
        _curHealth = _maxHealth;
        SetStatus(UnitStatus.IDLE);
        _animator = GetComponent<Animator>();
    }

    public void Update()
    {
        //we force the transition to DEATH, but everything else is defined in subclasses
        if (_curHealth < 0) SetStatus(UnitStatus.DEATH);

        switch (_status)
        {
            case UnitStatus.IDLE:
                UpdateIdle();
                break;
            case UnitStatus.WALKING:
                UpdateWalking();
                break;
            case UnitStatus.RUNNING:
                UpdateRunning();
                break;
            case UnitStatus.ATTACKING:
                UpdateAttacking();
                break;
            case UnitStatus.ATTACKRUNNING:
                UpdateAttackRunning();
                break;
            case UnitStatus.HEARTHSTONE:
                UpdateHearthStone();
                break;
            case UnitStatus.ABILITY1:
                UpdateAbility1();
                break;
            case UnitStatus.ABILITY2:
                UpdateAbility2();
                break;
            case UnitStatus.DEATH:
                UpdateDeath();
                break;
            default:
                Debug.LogError("unknown state " + _status.ToString());
                break;
        }

        _animator.SetInteger("UnitStatus", (int)_status);
    }

    abstract protected void UpdateIdle();
    abstract protected void UpdateWalking();
    abstract protected void UpdateRunning();
    abstract protected void UpdateAttacking();
    abstract protected void UpdateAttackRunning();
    abstract protected void UpdateHearthStone();
    abstract protected void UpdateAbility1();
    abstract protected void UpdateAbility2();
    abstract protected void UpdateDeath();

    /**
     * different types of units may choose to react differently to certain types of damage
     */
    virtual public bool ReceiveDamage(DamageType t, float damage)
    {
        _curHealth -= damage;
        return (_curHealth < 0);
    }

    protected Animator GetAnimatorComponent()
    {
        return _animator;
    }

    public float GetMaxHealth()
    {
        return _maxHealth;
    }

    public float GetCurrentHealth()
    {
        return _curHealth;
    }

    public Alliance GetAlliance()
    {
        return this._camp;
    }

    virtual public void ChooseSides(Alliance newAlliance)
    {
        this._camp = newAlliance;
        this.gameObject.BroadcastMessage("SetAlliance", newAlliance, SendMessageOptions.DontRequireReceiver);
    }

    public DamageType GetDamageType()
    {
        return this._damage;
    }

    public UnitStatus GetStatus()
    {
        return this._status;
    }

    public void SetStatus(UnitStatus newstatus)
    {
        this._status = newstatus;
    }

    private static bool isNull(MOBAUnit u)
    {
        return u == null;
    }
}

