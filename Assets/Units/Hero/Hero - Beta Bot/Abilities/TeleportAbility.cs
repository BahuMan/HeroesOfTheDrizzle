using UnityEngine;

public class TeleportAbility : TargettedAbility
{
    //this ability should get a target on the terrain, not any other unit
    public override int GetValidTargetMask()
    {
        return 1 << 8;
    }

    override public void AbilityStartEffect()
    {
        base.AbilityStartEffect();
        _hero.transform.position = _point;
    }
}

