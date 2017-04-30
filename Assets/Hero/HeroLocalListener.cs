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
    private GameObject marker;

    private const int TerrainLayerMask = 1 << 8;

	// Use this for initialization
	void Start () {
        _hero = GetComponent<HeroControl>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Ability1"))
        {
            _hero.ActivateAbility1();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 100f, TerrainLayerMask))
            {
                this.marker.transform.position = info.point;
                _hero.Attack(this.marker);
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
