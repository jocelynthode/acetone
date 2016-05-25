using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Range = Utils.Range;
using System.Collections.Generic;
using String = System.String;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int level;
    public Text donationText;
    public Text moneyGainText;
    public Text levelText;
    public AudioSource cashMoneyBiatch;
    public int moneyGain;
    public bool waitOnEnemiesAnimations;

    public BoardManager boardScript;

    public enum GameState
    {
        MENU,
        LEVELSETUP,
        PLAYERTURN,
        PLAYERMOVING,
        ENEMIESTURN,
        ENEMIESMOVING,
        UPGRADEMENU
    }

    public GameState state = GameState.MENU;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            CheckPlayerPrefs();
            DontDestroyOnLoad(gameObject);
            GameManager.instance.InitGame();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            if (instance.state == GameState.LEVELSETUP)
                instance.InitLevel();
            else
                instance.InitGame();
        }
    }

    void InitGame()
    {
        level = PlayerPrefs.GetInt("startGameLevel");
        moneyGain = 0;
        InitLevel();
    }

    void InitLevel()
    {
        levelText = GameObject.Find("levelText").GetComponent<Text>();
        int oldLevel = int.Parse(Regex.Match(levelText.text, @"\d+").Value);
        moneyGainText = GameObject.Find("moneyGainText").GetComponent<Text>();
        levelText.text = string.Format("Level: {0}", level);

        //Check if we changed the tens (e.g. : From 9 to 10 or from 15 to 21
        if (oldLevel / 10 != level / 10)
        {
            moneyGain += 3 * level; //TODO indicate this bonus
            moneyGain *= PlayerPrefs.GetInt("moneyGain", 0);
            int money = PlayerPrefs.GetInt("money");
            PlayerPrefs.SetInt("money", money + moneyGain);
            moneyGain = 0;
        }

        moneyGainText.text = string.Format("SKILL MONEY: ${0}", moneyGain);
        boardScript = GetComponent<BoardManager>();
        boardScript.SetupScene(level);
        donationText = GameObject.Find("donationText").GetComponent<Text>();
        state = GameState.PLAYERTURN;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.ENEMIESTURN:
                state = GameState.ENEMIESMOVING;
                StartCoroutine(MoveEnemies());
                break;
            default:
                break;
        }
    }

    public void OnLevelCompletion(int levelToPass = 1)
    {
        state = GameState.LEVELSETUP;
        StopAllCoroutines();
        level += levelToPass;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnTurnEnd()
    {
        state = GameState.ENEMIESTURN;
        float prob = Random.Range(1, 100);

        float viewersProb = Player.instance.TotalViewers / 250.0f;
        if (viewersProb > 30.0f)
            viewersProb = 30;

        if (prob <= (1.0f + viewersProb))
        {
            int money = PlayerPrefs.GetInt("money");
            int newMoney = (int)(0.1 * (new Range(10, 1500, 5f)).RandomInt());
            GainMoney("New Donation: ", money, newMoney);
        }
    }

    internal void RemoveDonation()
    {
        if (donationText)
            donationText.text = "";
    }

    public void GainMoney(string text, int oldMoney, int newMoney)
    {
        PlayerPrefs.SetInt("money", oldMoney + newMoney);
        DisplayText(string.Format(text + "{0:C2}", newMoney));
        Player.instance.money.text = string.Format("Money: ${0}", PlayerPrefs.GetInt("money"));
        cashMoneyBiatch.Play();

    }

    public void DisplayText(string text, int time=2)
    {
        donationText.text = text;
        Invoke("RemoveDonation", time);
    }

	public void OnGameOver(bool saveLevel = true)
    {
        state = GameState.LEVELSETUP;
        StopAllCoroutines();
        Destroy(boardScript.player.gameObject);
        //Set highestlevel attained ever
		if (saveLevel) {
			if (PlayerPrefs.GetInt ("highestLevel") < level) {
				PlayerPrefs.SetInt ("highestLevel", level);
			}
			SceneManager.LoadScene ("UpgradeMenu");	
			state = GameState.UPGRADEMENU;
		} else {
			SceneManager.LoadScene ("StartMenu");
			state = GameState.MENU;
		}
    }

    private IEnumerator MoveEnemies()
    {
		var enemies = boardScript.enemies;
        if (enemies.Count == 0)
        {
            state = GameState.PLAYERTURN;
            yield break;
        }

        // 1. Play all enemies turns (in the same frame)
        // 2. Move them back to their original position
        // 3. Replay all movements simultaneously and smoothly

        int maxActionsPerEnemy = enemies.Select(e => e.movesPerTurn).Max();
        List<EnemyAction>[] allActions = Enumerable.Range(0, maxActionsPerEnemy)
            .Select((_) => new List<EnemyAction>()).ToArray();
        
        foreach(var enemy in enemies)
        {
            for (int i = 0; i < enemy.movesPerTurn; i++)
            {
                var action = new EnemyAction() { enemy = enemy, origPosition = enemy.transform.position };

                waitOnEnemiesAnimations = false;
                enemy.Move();
                action.destPosition = enemy.transform.position;

                if (action.origPosition != action.destPosition)
                {
                    action.type = EnemyAction.Type.Move;
                    enemy.MoveRigidbody(action.origPosition);
                }
                else if (waitOnEnemiesAnimations)
                    action.type = EnemyAction.Type.Attack;
                else
                    action.type = EnemyAction.Type.None;

                allActions[i].Add(action);
            }
        };   

		foreach (List<EnemyAction> actions in allActions)
		{
			var coroutines = actions.Where(a => a.type == EnemyAction.Type.Move)
				.Select(action => StartCoroutine(action.enemy.SmoothMovement(action.destPosition)));
			foreach (var coroutine in coroutines.ToList())
				yield return coroutine;

			if (actions.Where(a => a.type == EnemyAction.Type.Attack).Count() > 0) {
				            // TODO use actual animation time
				            yield return new WaitForSeconds(0.6f);
			}
            // Manually wait if there are no enemies
            // TODO WAIT
		}

		state = GameState.PLAYERTURN;
    }

	private class EnemyAction
	{
		public Enemy enemy { get; set; }
		public Type type { get; set; }
		public Vector3 origPosition { get; set; } 
		public Vector3 destPosition { get; set; } 

		public enum Type {Move, Attack, None};
	}

    public static void CheckPlayerPrefs(bool force = false)
    {
        if (!force && PlayerPrefs.HasKey("money"))
            return;
        PlayerPrefs.SetInt("money", 30);

        PlayerPrefs.SetInt("attack", 10);
        PlayerPrefs.SetInt("defense", 5);
        PlayerPrefs.SetInt("maxHealth", 50);
        PlayerPrefs.SetInt("moneyGain", 0);
        PlayerPrefs.SetInt("startGameLevel", 1);
        PlayerPrefs.SetInt("viewers", 20);

        PlayerPrefs.SetInt("attackLevel", 0);
        PlayerPrefs.SetInt("defenseLevel", 0);
        PlayerPrefs.SetInt("maxHealthLevel", 0);
        PlayerPrefs.SetInt("startGameLevelLevel", 0);
        PlayerPrefs.SetInt("moneyGainLevel", 0);
        PlayerPrefs.SetInt("viewersLevel", 0);
        PlayerPrefs.SetInt("itemsPowerLevel", 0);
        PlayerPrefs.SetInt("tutorialDisplayed", 0);
    }
}
