using System.Collections;
using UnityEngine;

public class BaseControl : MOBAUnit {

    public delegate void BaseDestroyedEventHandler(BaseControl BaseKaputt);
    public event BaseDestroyedEventHandler BaseDestroyed;

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

    override protected void UpdateIdle() { /* base is always idle */ }
    override protected void UpdateWalking()       { Debug.LogError("Base " + gameObject.name + " shouldn't be walking"); }
    override protected void UpdateRunning()       { Debug.LogError("Base " + gameObject.name + " shouldn't be running"); }
    override protected void UpdateAttacking()     { Debug.LogError("Base " + gameObject.name + " shouldn't be attacking"); }
    override protected void UpdateAttackRunning() { Debug.LogError("Base " + gameObject.name + " shouldn't be attackrunning"); }
    override protected void UpdateHearthStone()   { Debug.LogError("Base " + gameObject.name + " shouldn't be casting hearthstone"); }
    override protected void UpdateAbility1()      { Debug.LogError("Base " + gameObject.name + " shouldn't be casting ability 1"); }
    override protected void UpdateAbility2()      { Debug.LogError("Base " + gameObject.name + " shouldn't be casting ability 2"); }
    override protected void UpdateDeath()
    {
        //@TODO: play crumbling animation?
        if (BaseDestroyed != null) BaseDestroyed(this);
    }

}
