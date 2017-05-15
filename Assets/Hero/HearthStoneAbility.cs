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
    public void CastHearthStoneSpell()
    {
        Debug.Log("Going Home");
        _hero.transform.position = _hearthStoneLocation.transform.position;
        _hero.transform.rotation = _hearthStoneLocation.transform.rotation;
        this.SetStatus(SkillStatus.COOLDOWN);
    }

    public void EndHearthStone()
    {
        Debug.Log("Gone Home");
        //previous attack status is probably no longer valid, so just returning to idle:
        _hero.SetStatus(MOBAUnit.UnitStatus.IDLE);
    }
}

