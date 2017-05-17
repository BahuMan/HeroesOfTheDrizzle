using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour {

    public BaseControl _baseBLUE;
    public BaseControl _baseRED;
    
    [SerializeField]
    private HeroControl[] _heroes;

	// Use this for initialization
	void Start () {
        HeroGUI g = GameObject.FindGameObjectWithTag("HeroGUI").GetComponent<HeroGUI>();
        g.SetLocalHero(_heroes[0]);
        StartCoroutine(DivideTeams());
	}

    public IEnumerator DivideTeams()
    {
        //just wait until next frame, when all Start() routines have been called
        yield return null;
        _heroes[0].SetHomeBase(_baseBLUE);
    }

}
