﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TurretControl : MOBAUnit {

    [SerializeField]
    private float damagePerAttack;
    [SerializeField]
    private float timeBetweenAttacks;

    [SerializeField]
    private GameObject _beam;
    private LineRenderer _laser;
    private Vector3 _laserOffset = Vector3.up * 1.5f;

    [SerializeField]
    private float _beamSmoothingTime = 1f;
    private Vector3 _beamSmoothingVelocity; 

    private float lastAttackTime;
    private MOBAUnit _targetEnemy;
    private EnemyTracker _enemyTracker;

    //audio related variables
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserSound;
    [SerializeField]
    private AudioClip _dieSound;
    [SerializeField]
    private AudioClip _discoverSound;

    override protected void Start () {
        base.Start();
        _beam.SetActive(false);
        _laser = GetComponent<LineRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _laser.SetPosition(0, this.transform.position +  _laserOffset*2);
        _laser.enabled = false;
        _enemyTracker = GetComponentInChildren<EnemyTracker>();
	}

    override protected void UpdateIdle()
    {
        if (_enemyTracker.GetNrEnemiesInSight() > 0)
        {
            _audioSource.PlayOneShot(_discoverSound);
            _targetEnemy = _enemyTracker.ChooseClosestEnemy();
            _beam.SetActive(true);
            _beam.transform.position = this.transform.position;
            SetStatus(UnitStatus.ATTACKING);
        }
    }

    override protected void UpdateAttacking()
    {

        //if we're out of enemies, return to idle
        
        if (_enemyTracker.GetNrEnemiesInSight() == 0)
        {
            //@TODO: perhaps animate the "return" of the search beam?
            _beam.SetActive(false);
            SetStatus(UnitStatus.IDLE);
            return;
        }

        //if current enemy is dead or gone, pick a new one:
        if (_targetEnemy == null || _targetEnemy.GetStatus() == UnitStatus.DEATH || !_enemyTracker.IsInSight(_targetEnemy))
        {
            //no need for dramatic sound effect, since the enemy has been in range for longer than just this frame:
            //_audioSource.PlayOneShot(_discoverSound);
            _targetEnemy = _enemyTracker.ChooseClosestEnemy();
        }

        //make the beam follow the target
        _beam.transform.position = Vector3.SmoothDamp(_beam.transform.position, _targetEnemy.transform.position, ref _beamSmoothingVelocity, _beamSmoothingTime);

        //deal damage in intervals:
        if (lastAttackTime + timeBetweenAttacks < Time.time)
        {
            //Debug.Log(gameObject.name + " firing at " + _targetEnemy.name);
            _audioSource.PlayOneShot(_laserSound);
            _laser.enabled = true;
            _laser.SetPosition(1, _targetEnemy.transform.position + _laserOffset);
            _targetEnemy.ReceiveDamage(GetDamageType(), this.damagePerAttack);
            lastAttackTime = Time.time;
            StartCoroutine(DeactivateLaser());
        }
    }

    override protected void UpdateWalking() { Debug.LogError(gameObject.name + "doesn't walk"); }
    override protected void UpdateRunning() { Debug.LogError(gameObject.name + "doesn't run"); }
    override protected void UpdateAttackRunning() { Debug.LogError(gameObject.name + "doesn't attackrun"); }
    override protected void UpdateHearthStone() { Debug.LogError(gameObject.name + "doesn't have HearthStone"); }
    override protected void UpdateAbility1() { Debug.LogError(gameObject.name + "doesn't have ability 1"); }
    override protected void UpdateAbility2() { Debug.LogError(gameObject.name + "doesn't have ability 2"); }

    private IEnumerator DeactivateLaser()
    {
        yield return new WaitForSeconds(0.1f);
        _laser.enabled = false;
    }

    override protected void UpdateDeath()
    {
        //Debug.Log(gameObject.name + " was destroyed");
        this._laser.enabled = false;
        this._beam.SetActive(false);
        _audioSource.PlayOneShot(_dieSound);
        Destroy(this); //immediate destruction of script to stop unit from doing anything
        Destroy(this.gameObject, 2); //delayed destruction to allow the sound to finish
    }

}
