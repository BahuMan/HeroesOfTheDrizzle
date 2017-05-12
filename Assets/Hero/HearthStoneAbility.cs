using UnityEngine;

[System.Serializable]
public class HearthStoneAbility: UntargettedAbility
{

    [SerializeField]
    private Transform _hearthStone;

    public HearthStoneAbility(HeroControl owner): base(owner, "HearthStone Ability")
    {
    }

    /**
     * SetHearthStone should be called by server when sides for players have been chosen
     */
    public void SetHearthStone(Transform loc)
    {
        _hearthStone = loc;
    }

    override protected void UpdateEffect()
    {
        base.UpdateEffect();
        _hero.transform.position = _hearthStone.transform.position;
        _hero.transform.rotation = _hearthStone.transform.rotation;
        this.SetStatus(SkillStatus.COOLDOWN);
    }

}

