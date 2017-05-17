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

    //private
    private HeroControl _localHero;
    private enum GUIStatus { NORMAL, ABILITY2SELECT }
    private GUIStatus status = GUIStatus.NORMAL;
    private int currentMask;

    private const int TerrainLayerMask = 1 << 8;
    private const int UnitsLayerMask = 1 << 9;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        _HealthSlider.value = _localHero.GetCurrentHealth();
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
                    _localHero.Attack(t);
                }
            }
        }

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

    public void SetLevelUpOptions(List<string> options)
    {
        _levelDropdown.ClearOptions();
        _levelDropdown.AddOptions(options);
    }
}
