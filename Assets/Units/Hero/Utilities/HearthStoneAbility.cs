using UnityEngine;

public class HearthStoneAbility : UntargettedAbility
{

    [SerializeField]
    private Transform _hearthStoneLocation;

    /**
     * SetHearthStone should be called by server/gamecontroller when sides for players have been chosen
     */
    public void SetHearthStone(Transform loc)
    {
        _hearthStoneLocation = loc;
    }

    override protected void UpdateEffect()
    {
        //base.UpdateEffect();
    }

    /**
     * This function is called from an event in the animation, right when the arms have fully spread
     */
    override public void AbilityStartEffect()
    {
        base.AbilityStartEffect();
        Debug.Log("Going Home");
        _hero.transform.position = _hearthStoneLocation.transform.position;
        _hero.transform.rotation = _hearthStoneLocation.transform.rotation;
        //skip straight to status "cooldown", because teleport is instantaneous
        this.SetStatus(SkillStatus.COOLDOWN);
    }

    public override void AbilityCastingFinished()
    {
        //contrary to "normal" untargetted ability, this time the hero *should* go back to IDLE
        _hero.SetStatus(MOBAUnit.UnitStatus.IDLE);
    }

}

