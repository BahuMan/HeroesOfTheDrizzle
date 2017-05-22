using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroGUI : MonoBehaviour {

    //FIELDS

    //references
    [SerializeField]
    private Texture2D Ability2Cursor;
    [SerializeField]
    private Dropdown _levelDropdown;
    [SerializeField]
    private Slider _HealthSlider;
    [SerializeField]
    private Slider _ManaSlider;
    [SerializeField]
    private Button HearthStoneButton;
    [SerializeField]
    private Button Ability1Button;
    [SerializeField]
    private Button Ability2Button;


    //private
    private Image HearthStoneButtonImg;
    private Image Ability1ButtonImg;
    private Image Ability2ButtonImg;
    private Image HealthFillImg;
    private Color HealthFillColor = Color.black;

    private HeroControl _localHero;
    private enum GUIStatus { NORMAL, ABILITY2SELECT }
    private GUIStatus status = GUIStatus.NORMAL;
    private int currentMask;

    private const int TerrainLayerMask = 1 << 8;
    private const int UnitsLayerMask = 1 << 9;

    // Use this for initialization
    void Start () {
        _levelDropdown.gameObject.SetActive(false);
        HearthStoneButtonImg = HearthStoneButton.GetComponent<Image>();
        Ability1ButtonImg = Ability1Button.GetComponent<Image>();
        Ability2ButtonImg = Ability2Button.GetComponent<Image>();
        HealthFillImg = _HealthSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
    }

// Update is called once per frame
void Update () {
        _HealthSlider.value = _localHero.GetCurrentHealth();
        HealthFillColor.r = (_localHero.GetMaxHealth() - _localHero.GetCurrentHealth() / _localHero.GetMaxHealth());
        HealthFillColor.g = _localHero.GetCurrentHealth() / _localHero.GetMaxHealth();
        HealthFillImg.color = HealthFillColor;
        _ManaSlider.value = _localHero.GetCurrentMana();

        if (Input.GetButtonDown("HearthStone"))
        {
            _localHero.ActivateHearthStone();
        }
        if (Input.GetButtonDown("Ability1"))
        {
            _localHero.ActivateAbility1();
        }
        if (Input.GetButtonDown("Ability2"))
        {
            SelectTargetForAbility2();
        }

        switch (status)
        {
            case GUIStatus.NORMAL:
                UpdateNormal();
                break;
            case GUIStatus.ABILITY2SELECT:
                UpdateAbility2Select();
                break;
        }
        CheckCoolDownButtons();
        CheckUpgradeChoices();

    }

    public void SelectTargetForAbility2()
    {
        //SetCursor generated a warning but does exactly what I expect it to do.
        Cursor.SetCursor(Ability2Cursor, new Vector2(32, 32), CursorMode.Auto);
        status = GUIStatus.ABILITY2SELECT;
        currentMask = _localHero.GetAbility2Mask();
    }

    private void UpdateAbility2Select()
    {
        //@TODO: change mouse cursor look based on target currently hovering
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;

            if (Physics.Raycast(ray, out info, 100f, currentMask))
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                //this.marker.transform.position = info.point;
                _localHero.ActivateAbility2(info.collider.gameObject);
                this.status = GUIStatus.NORMAL;
            }
            else
            {
                Debug.Log("Could not find a valid target for Ability 2");
            }
        }

        //right mouse button cancels ability select
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            status = GUIStatus.NORMAL;
        }
    }

    private void UpdateNormal()
    {
        /* disabling left mouse click reaction, so I can use the GUI without strange effects in the game:

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 100f, UnitsLayerMask))
            {
                MOBAUnit t = info.collider.GetComponent<MOBAUnit>();
                if (t == null)
                {
                    Debug.Log(info.collider.gameObject.name + " was not a moba unit?");
                }
                else
                {
                    Debug.Log("Ordering attack on " + t.name);
                    _localHero.Attack(t);
                }
            }
        }
        */

        if (Input.GetButtonDown("Fire2"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 100f, TerrainLayerMask))
            {
                //this.marker.transform.position = info.point;
                _localHero.MoveTo(info.point);
            }
        }

    }

    public void SetLocalHero(HeroControl hero)
    {
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
        SelectTargetForAbility2();
    }

    /**
     * This method will check the hero for a possible level-up.
     * If a choice is available, it will be presented to the player in the GUI.
     * By doing the polling from the HeroGUI class, none of the level-up mechanics
     * will be active for non-local players (e.g. when networking)
     */
    public void CheckUpgradeChoices()
    {
        //if a previous choice is still active, don't check for new choices:
        if (_levelDropdown.IsActive()) return;

        HeroUpgradeChoice[] choices = _localHero.GetUpgradeChoices();
        if (choices != null && choices.Length > 0)
        {
            //if a choice was offered by the hero, let's display it in the GUI:
            _levelDropdown.gameObject.SetActive(true);
            _levelDropdown.ClearOptions();
            //convert the list of options to something the GUI dropdown can read:
            List<string> opts = new List<string>(choices.Length+1);
            opts.Add("Choose Hero Upgrade");
            for (int c = 0; c < choices.Length; c++) opts.Add(choices[c].Option);
            _levelDropdown.AddOptions(opts);
        }
    }

    public void UpgradeChosen()
    {
        int c = _levelDropdown.value;
        Debug.Log("option chosen: " + c);
        if (c > 0)
        {
            //first line in the dropdown is "choose upgrade", so I need to subtract that line from the choice:
            _localHero.ChooseHeroUpgrade(c-1);
            _levelDropdown.gameObject.SetActive(false);
        }
    }

    //update the timer value for each of the ability buttons and update their looks accordingly
    private void CheckCoolDownButtons()
    {
        IndicateButton((Time.time - Mathf.Round(Time.time)), HearthStoneButton, HearthStoneButtonImg);
        IndicateButton(_localHero.GetAbility1CoolDown(), Ability1Button, Ability1ButtonImg);
        IndicateButton(_localHero.GetAbility2CoolDown(), Ability2Button, Ability2ButtonImg);
    }

    private void IndicateButton(float secondsLeft, Button b, Image i)
    {
        if (secondsLeft < float.Epsilon)
        {
            i.fillAmount = 1f; // Mathf.Clamp01((10f-a1)/10f);
            b.interactable = true;
        }
        else
        {
            i.fillAmount = Mathf.Clamp01((10f - secondsLeft) / 10f);
            b.interactable = false;
        }
    }
}
