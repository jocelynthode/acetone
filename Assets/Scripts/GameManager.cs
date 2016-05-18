using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Range = Utils.Range;
using System.Collections.Generic;
using String = System.String;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int level;
    public Text donationText;
    public Text moneyGainText;
    public Text levelText;
    public AudioSource cashMoneyBiatch;
    public int moneyGain;

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
        moneyGainText = GameObject.Find("moneyGainText").GetComponent<Text>();
        levelText.text = string.Format("Level: {0}", level);

        if (level % 10 == 0)
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

    public void OnLevelCompletion()
    {
        state = GameState.LEVELSETUP;
        StopAllCoroutines();
        level++;
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
        donationText.text = string.Format(text + "{0:C2}", newMoney);
        Player.instance.money.text = string.Format("Money: ${0}", PlayerPrefs.GetInt("money"));
        cashMoneyBiatch.Play();
        Invoke("RemoveDonation", 2);
    }

    public void OnGameOver()
    {
        state = GameState.LEVELSETUP;
        StopAllCoroutines();
        Destroy(boardScript.player.gameObject);
        //Set highestlevel attained ever
        if (PlayerPrefs.GetInt("highestLevel") < level)
        {
            PlayerPrefs.SetInt("highestLevel", level);
        }
        SceneManager.LoadScene("UpgradeMenu");	
        state = GameState.UPGRADEMENU;
    }

    private IEnumerator MoveEnemies()
    {
        var enemies = boardScript.enemies;

        // 1. Play all enemies turns (in the same frame)
        // 2. Move them back to their original position
        // 3. Replay all movements simultaneously and smoothly

        var originalPositions = new Dictionary<Enemy, Vector3>();
        var destPositions = new Dictionary<Enemy, Vector3>();
        enemies.ForEach(enemy =>
            {
                originalPositions.Add(enemy, enemy.transform.position);
                enemy.Move();
            });


        foreach (var enemy in enemies) {
            // Ignore enemies that didn't move
            if (enemy.transform.position != originalPositions[enemy])
            {
                destPositions.Add(enemy, enemy.transform.position);
                var orig = originalPositions[enemy];
                enemy.MoveRigidbody(orig);
            }
        }   

        // Do smooth movement

        // Manually wait if there are no enemies (the wait time will be shorter)
        if (destPositions.Count == 0)
        {
            if (enemies.Count > 0)
                yield return new WaitForSeconds(enemies[0].moveTime);
        }
        else
        {
            var coroutines = destPositions.Select(entry => StartCoroutine(entry.Key.SmoothMovement(entry.Value)));
            foreach (var coroutine in coroutines.ToList())
                yield return coroutine;
        }

        state = GameState.PLAYERTURN;
    }


    public static void CheckPlayerPrefs(bool force = false)
    {
        if (!force && PlayerPrefs.HasKey("money"))
            return;
        PlayerPrefs.SetInt("money", 100);

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
    }
}
