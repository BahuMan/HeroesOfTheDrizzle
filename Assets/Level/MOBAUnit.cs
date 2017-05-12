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
[RequireComponent(typeof(Collider))]
public abstract class MOBAUnit : MonoBehaviour
{

    public enum UnitStatus { IDLE, WALKING, RUNNING, ATTACKRUNNING, ATTACKING, HEARTHSTONE, ABILITY1, ABILITY2, DEATH }
    public enum DamageType { MELEE, FIRE, PIERCE, MAGIC, CRUSH }
    public enum Alliance { NEUTRAL, BLUE, RED }
    [SerializeField]
    private UnitStatus status;
    [SerializeField]
    private DamageType damage;
    [SerializeField]
    private Alliance camp;

    [SerializeField]
    private float maxHealth;
    private float curHealth;

    //references
    private Animator _animator;
    private HashSet<MOBAUnit> enemiesInSight;

    virtual protected void Start()
    {
        curHealth = maxHealth;
        status = UnitStatus.IDLE;
        _animator = GetComponent<Animator>();
        enemiesInSight = new HashSet<MOBAUnit>();
    }

    public void Update()
    {
        switch (status)
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
                Debug.LogError("unknown state " + status.ToString());
                break;
        }

        _animator.SetInteger("UnitStatus", (int)status);
        _animator.SetInteger("NrEnemies", GetNrEnemiesInSight());
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
        curHealth -= damage;
        return (curHealth < 0);
    }

    protected Animator GetAnimatorComponent()
    {
        return _animator;
    }

    public float GetHealth()
    {
        return curHealth;
    }

    public Alliance GetAlignment()
    {
        return this.camp;
    }

    public DamageType GetDamageType()
    {
        return this.damage;
    }

    public UnitStatus GetStatus()
    {
        return this.status;
    }

    protected void SetStatus(UnitStatus newstatus)
    {
        this.status = newstatus;
    }

    public int GetNrEnemiesInSight()
    {
        return this.enemiesInSight.Count;
    }

    protected IEnumerable<MOBAUnit> GetEnemiesInSight()
    {
        enemiesInSight.RemoveWhere(isNull);
        return this.enemiesInSight;
    }

    private static bool isNull(MOBAUnit u)
    {
        return u == null;
    }

    protected MOBAUnit ChooseClosestEnemy()
    {
        MOBAUnit closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (MOBAUnit enemy in GetEnemiesInSight())
        {
            //nullpointer check:
            //if (!enemy) continue;

            float d = (transform.position - enemy.transform.position).sqrMagnitude;
            if (d < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = d;
            }
        }
        return closestEnemy;
    }


    public void OnTriggerEnter(Collider other)
    {
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null && unit.GetAlignment() != this.GetAlignment())
        {
            enemiesInSight.Add(unit);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log(gameObject.name + " lost track of " + other.gameObject.name);
        MOBAUnit unit = other.gameObject.GetComponent<MOBAUnit>();
        if (unit != null)
        {
            enemiesInSight.Remove(unit);
        }
    }

}

