using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroGUI : MonoBehaviour {

    [SerializeField]
    private Dropdown _levelDropdown;

    [SerializeField]
    private Slider _HealthSlider;
    [SerializeField]
    private Slider _ManaSlider;

    private HeroControl _localHero;
    private HeroLocalListener _localHeroListener;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        _HealthSlider.value = _localHero.GetCurrentHealth();
        _ManaSlider.value = _localHero.GetCurrentMana();
	}

    public void SetLocalHero(HeroLocalListener heroListener, HeroControl hero)
    {
        _localHeroListener = heroListener;
        _localHero = hero;
        _HealthSlider.minValue = 0;
        _HealthSlider.maxValue = _localHero.GetMaxHealth();
        _ManaSlider.minValue = 0;
        _ManaSlider.maxValue = _localHero.GetMaxMana();
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

    public void SetLevelUpOptions(List<string> options)
    {
        _levelDropdown.ClearOptions();
        _levelDropdown.AddOptions(options);
    }
}
