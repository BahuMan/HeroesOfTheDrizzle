using UnityEngine;

public class FireBallAbility: TargettedAbility
{
    [SerializeField]
    private FireAndForget _fireBallPrefab;
    [SerializeField]
    private Vector3 _fireBallOffset = Vector3.up * 2;
    [SerializeField]
    private float _timeBetweenFireballs = .2f;

    //private FireAndForget _currentFireBall;
    private MOBAUnit _currentTarget;
    private float _lastFireballTime = 0;

    public override bool Activate(GameObject target, Vector3 point)
    {
        _currentTarget = target.GetComponent<MOBAUnit>(); //keep track of target, because we may need to guide fireball
        if (_currentTarget == null)
        {
            Debug.LogError("selected target " + target.name + " is not a MOBAUnit for FireBall");
            return false;
        }
        else if (_currentTarget.GetAlliance() == _hero.GetAlliance())
        {
            Debug.Log("Don't shoot your allies!");
            return false;
        }
        else
        {
            return base.Activate(target, point);
        }
    }


    /**
     * This method will be called by an event in the animation,
     * so we can time the moment the fireball is created.
     */
    override public void AbilityStartEffect()
    {
        base.AbilityStartEffect();
        Fire();
    }

    protected override void UpdateEffect()
    {
        base.UpdateEffect();
        if (_lastFireballTime + _timeBetweenFireballs < Time.time)
        {
            Fire();
        }
    }

    private void Fire()
    {
        FireAndForget missle = Instantiate<FireAndForget>(_fireBallPrefab, _hero.transform.position + _fireBallOffset, Quaternion.identity);
        missle.LaunchedBy = _hero;
        missle.Target = _currentTarget;
        _lastFireballTime = Time.time;
    }

}

