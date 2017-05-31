using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public BaseControl _baseBLUE;
    public BaseControl _baseRED;

    [SerializeField]
    private Text RedLevelText;
    [SerializeField]
    private Text BlueLevelText;
    [SerializeField]
    private Text GameOverText;
    
    [System.Serializable]
    public class PlayerInfo
    {
        public BaseControl PlayerHome;
        public HeroControl Prefab;
        public HeroControl CurrentHero;
        public bool IsLocal;

    }
    private Dictionary<HeroControl, PlayerInfo> playerList = new Dictionary<HeroControl, PlayerInfo>();

    private HeroGUI theGUI;

	// Use this for initialization
	void Start () {
        theGUI = GameObject.FindGameObjectWithTag("HeroGUI").GetComponent<HeroGUI>();
        CreatePlayers();

        if (_baseBLUE != null) _baseBLUE.BaseDestroyed += BaseDestroyed;
        if (_baseRED != null) _baseRED.BaseDestroyed += BaseDestroyed;
    }

    private void Update()
    {
        int redLevels = 0;
        int blueLevels = 0;
        foreach (PlayerInfo player in playerList.Values)
        {
            if (player.CurrentHero.GetAlliance() == MOBAUnit.Alliance.BLUE)
            {
                blueLevels += player.CurrentHero.GetPoints();
            }
            else if (player.CurrentHero.GetAlliance() == MOBAUnit.Alliance.RED)
            {
                redLevels += player.CurrentHero.GetPoints();
            }
        }
        RedLevelText.text = redLevels.ToString();
        BlueLevelText.text = blueLevels.ToString();
    }
    private void CreatePlayers()
    {

        //the StartMenuControl object comes from the StartMenu scene and contains options the player has chosen
        //before clicking the "play" button. These choices include the actual hero for the player and the computer.
        StartMenuControl startMenu = GameObject.FindObjectOfType<StartMenuControl>();
        if (!startMenu)
        {
            //if the testScene was run as-is, assume there's at least one hero in the level and couple it to the GUI
            foreach (HeroControl hero in GameObject.FindObjectsOfType<HeroControl>())
            {
                PlayerInfo info = new PlayerInfo();
                info.PlayerHome = hero.GetAlliance() == MOBAUnit.Alliance.BLUE? _baseBLUE : _baseRED;
                hero.SetHomeBase(info.PlayerHome);
                info.Prefab = null;
                info.CurrentHero = hero;
                info.IsLocal = true;
                info.CurrentHero.HeroDied += HeroDied; //call local method when this hero dies
                theGUI.SetLocalHero(info.CurrentHero);
                playerList.Add(info.CurrentHero, info);
            }
        }
        else
        {
            PlayerInfo info = new PlayerInfo();
            info.PlayerHome = _baseBLUE;
            info.Prefab = startMenu.choices[startMenu.PlayerChosen].Prefab;
            info.CurrentHero = Instantiate<HeroControl>(info.Prefab, _baseBLUE.SpawnPoint.position, _baseBLUE.SpawnPoint.rotation);
            info.IsLocal = true;
            info.CurrentHero.SetHomeBase(_baseBLUE);
            info.CurrentHero.HeroDied += HeroDied; //call local method when this hero dies
            theGUI.SetLocalHero(info.CurrentHero);
            playerList.Add(info.CurrentHero, info);

            PlayerInfo enemy = new PlayerInfo();
            enemy.PlayerHome = _baseRED;
            enemy.Prefab = startMenu.choices[startMenu.ComputerChosen].Prefab;
            enemy.CurrentHero = Instantiate<HeroControl>(enemy.Prefab, enemy.PlayerHome.SpawnPoint.position, enemy.PlayerHome.SpawnPoint.rotation);
            enemy.IsLocal = false;
            enemy.CurrentHero.SetHomeBase(_baseRED);
            enemy.CurrentHero.HeroDied += HeroDied; //call local method when this hero dies
            playerList.Add(enemy.CurrentHero, enemy);
        }
    }

    private void BaseDestroyed(BaseControl BaseKaputt)
    {
        GameOverText.text = "GAME OVER";
        GameOverText.gameObject.SetActive(true);

        foreach (PlayerInfo player in playerList.Values)
        {
            if (player.IsLocal)
            {
                if (BaseKaputt.GetAlliance() == player.PlayerHome.GetAlliance())
                {
                    GameOverText.text += "\nYou Lose";
                }
                else
                {
                    GameOverText.text += "\nYou Win!";
                }
            }
        }
        Invoke("GoBackToStartMenu", 5);
    }

    private void GoBackToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    /**
     * Delegate method that was registered with all heroes and will be called when the hero dies.
     * This will trigger a respawn with a basic hero
     */
    private void HeroDied(HeroControl deadHero)
    {
        PlayerInfo info = playerList[deadHero];
        if (info != null)
        {
            playerList.Remove(deadHero);
            info.CurrentHero = Instantiate<HeroControl>(info.Prefab, info.PlayerHome.SpawnPoint.position, info.PlayerHome.SpawnPoint.rotation);
            if (info.IsLocal) theGUI.SetLocalHero(info.CurrentHero);
            Debug.Log("Player died, respawning " + info.CurrentHero.name);
        }
        else
        {
            Debug.Log("Player died, no respawn");
        }
    }

}
