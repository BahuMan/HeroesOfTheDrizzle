using UnityEngine;

public class GameController : MonoBehaviour {

    public BaseControl _baseBLUE;
    public BaseControl _baseRED;

    [SerializeField]
    private HeroControl[] heroes;

	// Use this for initialization
	void Start () {
        heroes[0].SetHomeBase(_baseBLUE);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
