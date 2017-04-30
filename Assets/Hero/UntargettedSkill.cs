using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UntargettedSkill {

    [SerializeField]
    private ParticleSystem effect;
    [SerializeField]
    private float CastDuration = 1f;
    [SerializeField]
    private float EffectDuration = 2f;
    [SerializeField]
    private float CoolDownTime = 2f;

    private float _currentDuration = -1f;
    private float _currentCoolDown = -1f;
    private bool _active = false;

    public bool Activate()
    {
        if (_active || !IsAvailable())
        {
            Debug.Log("Hero can't activate ability1: timer is " + _currentCoolDown);
            return false; //can't activate (yet)
        }

        Debug.Log("Hero activated special ability 1");
        if (effect) effect.Play();
        _currentDuration = EffectDuration;
        _active = true;
        _currentCoolDown = CoolDownTime;
        return true;
    }

    public void DeActivate()
    {
        if (!_active) return;

        _active = false;
        if (effect) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        //_currentCoolDown = _coolDownTime; //already set during activation
        Debug.Log("Hero deactivated special ability 1");
    }

    /**
     * should be called every frame, to allow for continuous or delayed effects and animations
     */
    public void SkillUpdate()
    {
        if (_active)
        {
            _currentDuration -= Time.deltaTime;
            if (_currentDuration < 0) DeActivate();
        }
        else if (_currentCoolDown > 0)
        {
            _currentCoolDown -= Time.deltaTime;
        }
    }

    public bool IsActive()
    {
        return _active;
    }

    public bool IsAvailable()
    {
        return _currentCoolDown < 0;
    }

    public float GetCurrentCoolDownTime()
    {
        return _currentCoolDown;
    }


}
