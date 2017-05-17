using UnityEngine;

public class FireBall: TargettedAbility
{
    [SerializeField]
    private ParticleSystem FireBallPrefab;

    private ParticleSystem _currentFireBall;
    private MOBAUnit _currentTarget;

    public override bool Activate(GameObject target)
    {
        _currentTarget = target.GetComponent<MOBAUnit>(); //keep track of target, because we may need to guide fireball
        if (_currentTarget != null)
        {
            Debug.LogError("selected target " + target.name + " is not a MOBAUnit for FireBall");
            return false;
        }
        else if (_currentTarget != null && base.Activate(target))
        {
            //@TODO: remove this method call as soon as animation has an event
            throwFireBall();
            return true;
        }
        else return false;
    }

    protected override void UpdateEffect()
    {
        base.UpdateEffect();

        //if the fireball is still flying, we direct it to the target
        if (!_currentFireBall)
        {
            //@TODO: verify that fireball location changes as I expect
        }
    }

    /**
     * This method will be called by an event in the animation,
     * so we can time the moment the fireball is created.
     */
    private void throwFireBall()
    {
        Vector3 direction = _currentTarget.transform.position - _hero.transform.position;
        direction.y = 0;
        direction.Normalize();
        Quaternion rot = Quaternion.LookRotation(direction);
        _currentFireBall = Instantiate<ParticleSystem>(FireBallPrefab, _hero.transform.position, rot);
    }
}

