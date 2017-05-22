﻿using System.Collections;
using UnityEngine;

public class BaseControl : MonoBehaviour {

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
    [SerializeField]
    private MOBAUnit.Alliance camp;


	// Use this for initialization
	void Start () {
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
                    Debug.Log(gameObject.name + " spawning minion " + minion.name);
                    minion.setWaypoints(l.WayPoints);
                    minion.ChooseSides(this.camp);
                    yield return new WaitForSeconds(IntervalBetweenMinions);
                }
            }
            yield return new WaitForSeconds(IntervalBetweenLaunches);
        }
    }

    public MOBAUnit.Alliance getAlliance()
    {
        return this.camp;
    }
}
