using System.Collections;
using UnityEngine;

public class BaseControl : MOBAUnit {

    //event handler so the GameController will receive a callback when any base has been destroyed (end of game)
    public delegate void BaseDestroyedEventHandler(BaseControl BaseKaputt);
    public event BaseDestroyedEventHandler BaseDestroyed;

    //every base will heal nearby allied units:
    [SerializeField]
    private AllyTracker _allyTracker;
    [SerializeField]
    private float _healAmount = 5f;
    [SerializeField]
    private float _healTime = 1f;
    private float _lastHealTime = 0f;

    //physics & particle effects for the destroyed base:
    [SerializeField]
    DestroyExplosion BaseDestroyedPrefab;

    //references:
    public Transform SpawnPoint;
    [SerializeField]
    private Lane[] lanes;
    [SerializeField]
    private MinionAI minionPrefab;
    [SerializeField]
    private int nrMinionsToLaunch = 2;
    [SerializeField]
    private float IntervalBetweenLaunches = 30;
    [SerializeField]
    private float IntervalBetweenMinions = .5f;


	// Use this for initialization
    override protected void Start () {
        base.Start();
        StartCoroutine(LaunchMinions());
	}
	
    private IEnumerator LaunchMinions()
    {
        while (true)
        {
            foreach (var l in lanes)
            {
                for (int m=0; m<nrMinionsToLaunch; ++m)
                {
                    MinionAI minion = Instantiate<MinionAI>(minionPrefab, SpawnPoint.position, SpawnPoint.rotation);
                    //Debug.Log(gameObject.name + " spawning minion " + minion.name);
                    minion.setWaypoints(l.WayPoints);
                    minion.ChooseSides(this.GetAlliance());
                    yield return new WaitForSeconds(IntervalBetweenMinions);
                }
            }
            yield return new WaitForSeconds(IntervalBetweenLaunches);
        }
    }

    override protected void UpdateIdle() { HealNearbyAllies(); }
    override protected void UpdateWalking()       { Debug.LogError("Base " + gameObject.name + " shouldn't be walking"); }
    override protected void UpdateRunning()       { Debug.LogError("Base " + gameObject.name + " shouldn't be running"); }
    override protected void UpdateAttacking()     { Debug.LogError("Base " + gameObject.name + " shouldn't be attacking"); }
    override protected void UpdateAttackRunning() { Debug.LogError("Base " + gameObject.name + " shouldn't be attackrunning"); }
    override protected void UpdateHearthStone()   { Debug.LogError("Base " + gameObject.name + " shouldn't be casting hearthstone"); }
    override protected void UpdateAbility1()      { Debug.LogError("Base " + gameObject.name + " shouldn't be casting ability 1"); }
    override protected void UpdateAbility2()      { Debug.LogError("Base " + gameObject.name + " shouldn't be casting ability 2"); }

    private void HealNearbyAllies()
    {
        if (_lastHealTime + _healTime > Time.time) return;
        _lastHealTime = Time.time;


        foreach (MOBAUnit friend in _allyTracker.GetAlliesInSight())
        {
            if (friend.GetCurrentHealth() < friend.GetMaxHealth())
            {
                Debug.Log("Base healing " + friend.name);
                friend.ReceiveDamage(DamageType.MAGIC, -_healAmount);
            }
        }
    }

    override protected void UpdateDeath()
    {
        //@TODO: play crumbling animation?
        if (BaseDestroyed != null) BaseDestroyed(this);

        Destroy(this.gameObject);

        DestroyExplosion exp = Instantiate<DestroyExplosion>(this.BaseDestroyedPrefab, transform.position, transform.rotation);
        exp.Boom();

    }

}
