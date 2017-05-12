using UnityEngine;
using UnityEngine.UI;

public class HeroGUI : MonoBehaviour {

    [SerializeField]
    private Dropdown _skillDropdown;

    private HeroControl _localHero;
    private HeroLocalListener _localHeroListener;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetLocalHero(HeroLocalListener heroListener, HeroControl hero)
    {
        _localHeroListener = heroListener;
        _localHero = hero;
    }

    public void ClickedHearthStone()
    {
        _localHero.ActivateHearthStone();
    }

    public void ClickedAbility1()
    {
        _localHero.ActivateAbility1();
    }

    public void ClickedAbility2()
    {
        _localHeroListener.SelectTargetForAbility2();
    }
}
