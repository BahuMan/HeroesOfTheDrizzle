using UnityEngine;

public class TurretControl : MOBAUnit {

    [SerializeField]
    private float damagePerAttack;
    [SerializeField]
    private float timeBetweenAttacks;

    private float lastAttackTime;
    private MOBAUnit _targetEnemy;

    override protected void Start () {
        base.Start();
	}

    override protected void UpdateIdle()
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

    override protected void UpdateAttacking()
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

    override protected void UpdateWalking() { Debug.LogError(gameObject.name + "doesn't walk"); }
    override protected void UpdateRunning() { Debug.LogError(gameObject.name + "doesn't run"); }
    override protected void UpdateAttackRunning() { Debug.LogError(gameObject.name + "doesn't attackrun"); }
    override protected void UpdateAbility1() { Debug.LogError(gameObject.name + "doesn't have ability 1"); }
    override protected void UpdateAbility2() { Debug.LogError(gameObject.name + "doesn't have ability 2"); }

    override protected void UpdateDeath()
    {
        Debug.Log(gameObject.name + " was destroyed");
        Destroy(this.gameObject);
    }

}
