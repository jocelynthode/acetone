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
    private int oldLevel;
    public int level;
    public Text donationText;
    public Text levelText;
    public int moneyGain;
    public bool waitOnEnemiesAnimations;
    private Vector3 playerLastPosition;
    public AStar aStar;

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
        oldLevel = level;
        moneyGain = 0;
        InitLevel();
    }

    void InitLevel()
    {
        levelText = GameObject.Find("levelText").GetComponent<Text>();
        levelText.text = string.Format("Level: {0}", level);
        donationText = GameObject.Find("donationText").GetComponent<Text>();

        //Check if we changed the tens (e.g. : From 9 to 10 or from 15 to 21
        // we don't want to pay money on first level
        if (oldLevel != 1 && oldLevel / 10 != level / 10 )
        {
            moneyGain += (3+PlayerPrefs.GetInt("moneyGain")) * level; //TODO indicate this bonus
            int money = PlayerPrefs.GetInt("money");
            PlayerPrefs.SetInt("money", money + moneyGain);
            moneyGain = 0;
        }
        if (level - oldLevel > 1)
            DisplayText(String.Format("{0} levels skipped!", level - oldLevel));

        oldLevel = level;

        boardScript = GetComponent<BoardManager>();
        boardScript.SetupScene(level);
        aStar = new AStar();

        state = GameState.PLAYERTURN;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.PLAYERTURN:
                // Make sure money is updated after level change.
                // We can't do it in InitLevel() because player.instance may not be set yet.
                Player.instance.RefreshUI();
                break;
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
        var oldPosition = playerLastPosition;
        playerLastPosition = Player.instance.transform.position;
        // No donation if the player didn't move
        if (oldPosition == playerLastPosition)
            return;

        float prob = Random.Range(1, 100);

        float viewersProb = Player.instance.TotalViewers / 250.0f;
        if (viewersProb > 30.0f)
            viewersProb = 30;

        if (prob <= (1.0f + viewersProb))
        {
            int money = PlayerPrefs.GetInt("money");
            int newMoney = (int)(0.1 * (new Range(10, 400, 5f)).RandomInt());
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
        DisplayText(string.Format(text + "${0}", newMoney));
        Player.instance.RefreshUI();
        Utils.PlaySound("cashtill");
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
        List<EnemyAction>[] allActions = new List<EnemyAction>[maxActionsPerEnemy];

        for (int i = 0; i < maxActionsPerEnemy; i++)
        {
            allActions[i] = enemies.Where(e => e.movesPerTurn > i).Select(enemy => {
                var action = new EnemyAction() { enemy = enemy, origPosition = enemy.transform.position };

                waitOnEnemiesAnimations = false;
                enemy.Move();
                action.destPosition = enemy.transform.position;

                if (action.origPosition != action.destPosition)
                    action.type = EnemyAction.Type.Move;
                else if (waitOnEnemiesAnimations)
                    action.type = EnemyAction.Type.Attack;
                else
                    action.type = EnemyAction.Type.None;

                return action;
            }).ToList();
        }

        allActions[0].ForEach(action => action.enemy.MoveRigidbody(action.origPosition));

		foreach (List<EnemyAction> actions in allActions)
		{
            var coroutines = actions.Where(a => a.type == EnemyAction.Type.Move)
                .Select(action => StartCoroutine(action.enemy.SmoothMovement(action.destPosition)));
            foreach (var coroutine in coroutines.ToList())
				yield return coroutine;

			if (actions.Where(a => a.type == EnemyAction.Type.Attack).Count() > 0) {
                // TODO use actual animation time
                yield return new WaitForSeconds(0.6f);
            } else if (actions.Count > 0 && actions.Where(a => a.type == EnemyAction.Type.None).Count() == actions.Count) {
                float waitTime = actions.Select(a => a.enemy.moveTime).Max();
                yield return new WaitForSeconds(waitTime);
            }
		}

		state = GameState.PLAYERTURN;
    }

	private class EnemyAction
	{
        public Enemy enemy;
        public Type type;
        public Vector3 origPosition;
        public Vector3 destPosition;

		public enum Type {Move, Attack, None};
	}

    public static void CheckPlayerPrefs(bool force = false)
    {
        if (!force && PlayerPrefs.HasKey("money"))
            return;
        PlayerPrefs.SetInt("money", 30);

        PlayerPrefs.SetInt("attack", 10);
        PlayerPrefs.SetInt("maxHealth", 100);
        PlayerPrefs.SetInt("moneyGain", 1);
        PlayerPrefs.SetInt("startGameLevel", 1);
        PlayerPrefs.SetInt("viewers", 20);

        PlayerPrefs.SetInt("attackLevel", 0);
        PlayerPrefs.SetInt("maxHealthLevel", 0);
        PlayerPrefs.SetInt("startGameLevelLevel", 0);
        PlayerPrefs.SetInt("moneyGainLevel", 0);
        PlayerPrefs.SetInt("viewersLevel", 0);
        PlayerPrefs.SetInt("itemsPowerLevel", 0);
        PlayerPrefs.SetInt("tutorialDisplayed", 0);
    }
}
