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

    public enum UnitStatus { IDLE, WALKING, RUNNING, ATTACKRUNNING, ATTACKING, ABILITY1, ABILITY2, DEATH }
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

    virtual public void Start()
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

    abstract public void UpdateIdle();
    abstract public void UpdateWalking();
    abstract public void UpdateRunning();
    abstract public void UpdateAttacking();
    abstract public void UpdateAttackRunning();
    abstract public void UpdateAbility1();
    abstract public void UpdateAbility2();
    abstract public void UpdateDeath();

    /**
     * different types of units may choose to react differently to certain types of damage
     */
    virtual public bool ReceiveDamage(DamageType t, float damage)
    {
        curHealth -= damage;
        return (curHealth < 0);
    }

    public Animator GetAnimatorComponent()
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

    public void SetStatus(UnitStatus newstatus)
    {
        this.status = newstatus;
    }

    public int GetNrEnemiesInSight()
    {
        return this.enemiesInSight.Count;
    }

    public IEnumerable<MOBAUnit> GetEnemiesInSight()
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

