using UnityEngine;

/**
 * base class for everything that can be attacked (buildings, units, minions, heroes)
 */
public class MOBAUnit: MonoBehaviour
{

    [SerializeField]
    private float maxHealth;
    private float curHealth;

    public enum DamageType { MELEE, FIRE, PIERCE, MAGIC, CRUSH}
    public enum Alliance {NEUTRAL, BLUE, RED}
    [SerializeField]
    private Alliance camp;

    virtual public void Start()
    {
        Debug.Log("MOBAUnity start for " + gameObject.name);
        curHealth = maxHealth;
    }

    public float GetHealth()
    {
        return curHealth;
    }

    /**
     * different types of units may choose to react differently to certain types of damage
     */
    virtual public bool ReceiveDamage(DamageType t, float damage)
    {
        curHealth -= damage;
        return (curHealth < 0);
    }

    public Alliance GetAlignment()
    {
        return this.camp;
    }
}

