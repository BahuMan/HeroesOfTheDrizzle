using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuControl : MonoBehaviour {

    [System.Serializable]
    public class HeroChoice
    {
        public string Name;
        public HeroControl Prefab;
    }

    public HeroChoice[] choices;
    public string[] sceneNames;

    [SerializeField]
    private Dropdown MapDropDown;
    [SerializeField]
    private Dropdown HeroDropDown;
    [SerializeField]
    private Dropdown ComputerDropDown;

    public int PlayerChosen;   //value of the HeroDropDown     will be stored here as we load a new scene
    public int ComputerChosen; //value of hte ComputerDropDown will be stored here as we load a new scene

    // Use this for initialization
    void Start () {

        //fill the dropdowns for player hero and computer hero with the available choices:

        List<string> opts = new List<string>(choices.Length);
		for (int i=0; i<choices.Length; ++i)
        {
            opts.Add(choices[i].Name);
        }

        HeroDropDown.ClearOptions();
        HeroDropDown.AddOptions(opts);

        ComputerDropDown.ClearOptions();
        ComputerDropDown.AddOptions(opts);

	}
	

    public void HeroChoiceChanged()
    {
        //temp code so the computer will select whatever hero the player has NOT selected:
        if (HeroDropDown.value == 0)
        {
            ComputerDropDown.value = 1;
            ComputerDropDown.RefreshShownValue();
        }
        else if (HeroDropDown.value == 1)
        {
            ComputerDropDown.value = 0;
            ComputerDropDown.RefreshShownValue();
        }
        else
        {
            Debug.LogError("more options than I anticipated ?!");
        }
    }

    public void PlayButtonClicked()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        this.PlayerChosen = HeroDropDown.value;
        this.ComputerChosen = ComputerDropDown.value;

        SceneManager.LoadScene(sceneNames[MapDropDown.value]);
    }
}
