using UnityEngine;

public class FireBallAbility: TargettedAbility
{
    [SerializeField]
    private FireAndForget _fireBallPrefab;
    [SerializeField]
    private Vector3 _fireBallOffset = Vector3.up * 2;

    //private FireAndForget _currentFireBall;
    private MOBAUnit _currentTarget;

    public override bool Activate(GameObject target)
    {
        _currentTarget = target.GetComponent<MOBAUnit>(); //keep track of target, because we may need to guide fireball
        if (_currentTarget == null)
        {
            Debug.LogError("selected target " + target.name + " is not a MOBAUnit for FireBall");
            return false;
        }
        else if (base.Activate(target))
        {
            return true;
        }
        else return false;
    }


    /**
     * This method will be called by an event in the animation,
     * so we can time the moment the fireball is created.
     */
    override public void AbilityStartEffect()
    {
        base.AbilityStartEffect();
        //Vector3 direction = _currentTarget.transform.position - _hero.transform.position;
        //direction.y = 0;
        //direction.Normalize();
        //Quaternion rot = Quaternion.LookRotation(direction);
        FireAndForget missle = Instantiate<FireAndForget>(_fireBallPrefab, _hero.transform.position + _fireBallOffset, Quaternion.identity);
        missle.Target = _currentTarget;
    }
}

