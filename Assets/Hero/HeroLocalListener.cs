using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * All keyboard and mousecontrol for the hero is concentrated in this class.
 * This is closer to the MVC pattern and hopefully allows networked multiplayer more easily
 */
[RequireComponent(typeof(HeroControl))]
public class HeroLocalListener : MonoBehaviour {

    private HeroControl _hero;
    [SerializeField]
    private Texture2D Ability2Cursor;

    [SerializeField]
    private GameObject marker;

    private HeroGUI _heroGUI;
    private enum GUIStatus {NORMAL, ABILITY2SELECT}
    private GUIStatus status = GUIStatus.NORMAL;
    private int currentMask;

    private const int TerrainLayerMask = 1 << 8;
    private const int EnemiesLayerMask = 1 << 9;

	// Use this for initialization
	void Start () {
        _hero = GetComponent<HeroControl>();
        GameObject go = GameObject.FindGameObjectWithTag("HeroGUI");
        _heroGUI = go.GetComponent<HeroGUI>();
        _heroGUI.SetLocalHero(this, _hero);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Ability1"))
        {
            _hero.ActivateAbility1();
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

    public void ChooseBase(BaseControl homeBase)
    {
        _hero.SetHomeBase(homeBase);
    }

    public void SelectTargetForAbility2()
    {
        //SetCursor generated a warning but does exactly what I expect it to do.
        Cursor.SetCursor(Ability2Cursor, new Vector2(32, 32), CursorMode.Auto);
        status = GUIStatus.ABILITY2SELECT;
        currentMask = _hero.GetAbility2Mask();
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
                _hero.ActivateAbility2(info.collider.gameObject);
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
            if (Physics.Raycast(ray, out info, 100f, EnemiesLayerMask))
            {
                MOBAUnit t = info.collider.GetComponent<MOBAUnit>();
                if (t == null)
                {
                    Debug.Log(info.collider.gameObject.name + " was not a moba unit?");
                }
                else
                {
                    _hero.Attack(t);
                }
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 100f, TerrainLayerMask))
            {
                this.marker.transform.position = info.point;
                _hero.MoveTo(this.marker.transform);
            }
        }
    }
}
