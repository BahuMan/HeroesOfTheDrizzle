using UnityEngine;

public class GameController : MonoBehaviour {

    public BaseControl _baseBLUE;
    public BaseControl _baseRED;
    
    [SerializeField]
    private HeroControl[] _heroes;

	// Use this for initialization
	void Start () {
        _heroes[0].SetHomeBase(_baseBLUE);

        HeroGUI g = GameObject.FindGameObjectWithTag("HeroGUI").GetComponent<HeroGUI>();
        g.SetLocalHero(_heroes[0]);
	}
	
}
