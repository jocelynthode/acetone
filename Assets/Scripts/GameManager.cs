using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public int level;
    public Text donationText;
    public AudioSource cashMoneyBiatch;

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
    void Awake () {
        if (instance == null)
        {
            instance = this;
            CheckPlayerPrefs();
            DontDestroyOnLoad(gameObject);
            GameManager.instance.InitGame();
        } else if (instance != this)
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
		level = PlayerPrefs.GetInt ("level",1);
        InitLevel();
    }

    void InitLevel()
    {
        boardScript = GetComponent<BoardManager>();
        boardScript.SetupScene(level);
        donationText = GameObject.Find("donationText").GetComponent<Text>();
        state = GameState.PLAYERTURN;
    }

	// Update is called once per frame
	void Update () {
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
        StopCoroutine(enemiesCoroutine);
        level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // http://forum.unity3d.com/threads/random-range-with-decreasing-probability.50596/
    float SkewedRandomRange(float start, float end, float p)
    {
        return Mathf.Pow(Random.value, p) * (end - start) + start;
    }


    public void OnTurnEnd()
    {
        state = GameState.ENEMIESTURN;
        float prob = Random.Range(1, 100);
        int viewers = PlayerPrefs.GetInt("viewers");
        float viewersProb = viewers / 250.0f;
        if (viewersProb > 30.0f) viewersProb = 30;
        if (prob > (10.0f + viewersProb)) return;
        int money = PlayerPrefs.GetInt("money");
        
        int newMoney = (int) (0.1 * SkewedRandomRange(10,1500,5f));
        PlayerPrefs.SetInt("money", money + newMoney);
        donationText.text = string.Format("New Donation: {0:C2}", newMoney);
        Player.instance.money.text = string.Format("Money: {0:C2}", PlayerPrefs.GetInt("money", 0));
        cashMoneyBiatch.Play();
        Invoke("RemoveDonation", 2);
    }

    internal void RemoveDonation()
    {
        if(donationText)
            donationText.text = "";
    }

    public void OnGameOver()
    {
        state = GameState.LEVELSETUP;
        StopCoroutine(enemiesCoroutine);
        Destroy(boardScript.player.gameObject);
        SceneManager.LoadScene("UpgradeMenu");
        state = GameState.UPGRADEMENU;
    }

    private IEnumerator MoveEnemies()
    {
        yield return new WaitForSeconds(0.1f);
        foreach(Enemy enemy in boardScript.enemies)
        {
            enemy.Move();
            yield return new WaitForSeconds(enemy.moveTime);
        }
        state = GameState.PLAYERTURN;
    }
    
    public static void CheckPlayerPrefs(bool force = false)
    {
        if (!force && PlayerPrefs.HasKey("maxHealth")) return;
        PlayerPrefs.SetInt("attack", 10);
        PlayerPrefs.SetInt("defense", 5);
        PlayerPrefs.SetInt("maxHealth", 50);
        PlayerPrefs.SetInt("money", 100);
        PlayerPrefs.SetInt("viewers", 20);
        PlayerPrefs.SetInt("moneyGain", 0);
        PlayerPrefs.SetInt("attackLevel", 0);
        PlayerPrefs.SetInt("defenseLevel", 0);
        PlayerPrefs.SetInt("moneyGainLevel", 0);
        PlayerPrefs.SetInt("viewersLevel", 0);
    }
}
