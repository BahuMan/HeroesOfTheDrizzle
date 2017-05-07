using UnityEngine;

public class TurretControl : MOBAUnit {

    [SerializeField]
    private float damagePerAttack;
    [SerializeField]
    private float timeBetweenAttacks;

    private float lastAttackTime;
    private MOBAUnit _targetEnemy;

    override public void Start () {
        base.Start();
	}

    override public void UpdateIdle()
    {
        if (GetHealth() < 0)
        {
            SetStatus(UnitStatus.DEATH);
        }
        else if (GetNrEnemiesInSight() > 0)
        {
            _targetEnemy = ChooseClosestEnemy();
            SetStatus(UnitStatus.ATTACKING);
        }
    }

    override public void UpdateAttacking()
    {
        if (_targetEnemy == null)
        {
            SetStatus(UnitStatus.IDLE);
            return;
        }
        if (lastAttackTime + timeBetweenAttacks < Time.time)
        {
            Debug.Log(gameObject.name + " firing at " + _targetEnemy.name);
            _targetEnemy.ReceiveDamage(GetDamageType(), this.damagePerAttack);
            lastAttackTime = Time.time;
        }
    }

    override public void UpdateWalking() { Debug.LogError(gameObject.name + "doesn't walk"); }
    override public void UpdateRunning() { Debug.LogError(gameObject.name + "doesn't run"); }
    override public void UpdateAttackRunning() { Debug.LogError(gameObject.name + "doesn't attackrun"); }
    override public void UpdateAbility1() { Debug.LogError(gameObject.name + "doesn't have ability 1"); }
    override public void UpdateAbility2() { Debug.LogError(gameObject.name + "doesn't have ability 2"); }

    override public void UpdateDeath()
    {
        Debug.Log(gameObject.name + " was destroyed");
        Destroy(this.gameObject);
    }

}
