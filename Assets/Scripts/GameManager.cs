using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Range = Utils.Range;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int level;
    public Text donationText;
    public Text moneyGainText;
    public Text levelText;
    public AudioSource cashMoneyBiatch;
    public int moneyGain;

    private IEnumerator enemiesCoroutine;

    public BoardManager boardScript;

    public enum GameState
    {
        MENU,
        LEVELSETUP,
        PLAYERTURN,
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
                enemiesCoroutine = MoveEnemies();
                StartCoroutine(enemiesCoroutine);
                break;
            default:
                break;
        }
    }

    public void OnLevelCompletion()
    {
        state = GameState.LEVELSETUP;
        if (enemiesCoroutine != null)
            StopCoroutine(enemiesCoroutine);
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
        if (enemiesCoroutine != null)
            StopCoroutine(enemiesCoroutine);
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
        yield return new WaitForSeconds(0.1f);
        foreach (Enemy enemy in boardScript.enemies)
        {
            enemy.Move();
            yield return new WaitForSeconds(enemy.moveTime);
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
