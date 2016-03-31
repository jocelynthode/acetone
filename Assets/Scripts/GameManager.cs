using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public bool playerTurn;
    private bool enemiesMoving;
    public int level;
    public bool levelSetup = true;  // Probably useless
    public Text donationText;
    public AudioSource cashMoneyBiatch;

    private IEnumerator enemiesCoroutine;

    public BoardManager boardScript;

	// Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
            CheckPlayerPrefs();
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        GameManager.instance.InitGame();
    }

    void InitGame()
    {
        playerTurn = true;
        enemiesMoving = false;
        boardScript.SetupScene(level);
        levelSetup = false;
        donationText = GameObject.Find("donationText").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        if (playerTurn || enemiesMoving || levelSetup) return;
        enemiesCoroutine = MoveEnemies();
        StartCoroutine(enemiesCoroutine);
	}

    public void OnLevelCompletion()
    {
        levelSetup = true;
        StopCoroutine(enemiesCoroutine);
        // Show upgrade screen
        level++;
        // InitGame();
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // http://forum.unity3d.com/threads/random-range-with-decreasing-probability.50596/
    float SkewedRandomRange(float start, float end, float p)
    {
        return Mathf.Pow(Random.value, p) * (end - start) + start;
    }


    public void OnTurnEnd()
    {
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
        StopCoroutine(enemiesCoroutine);
        Destroy(boardScript.player.gameObject);
        SceneManager.LoadScene("UpgradeMenu");
    }

    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(0.1f);
        foreach(Enemy enemy in boardScript.enemies)
        {
            enemy.Move();
            yield return new WaitForSeconds(enemy.moveTime);
        }
        playerTurn = true;
        enemiesMoving = false;
    }
    
    public void CheckPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("maxhealth")) return;
        PlayerPrefs.SetInt("attack", 10);
        PlayerPrefs.SetInt("defense", 5);
        PlayerPrefs.SetInt("maxhealth", 50);
        PlayerPrefs.SetInt("money", 0);
        PlayerPrefs.SetInt("viewers", 20);
        PlayerPrefs.SetInt("moneyGain", 0);
        PlayerPrefs.SetInt("attackLevel", 0);
        PlayerPrefs.SetInt("defenseLevel", 0);
        PlayerPrefs.SetInt("moneyGainLevel", 0);
        PlayerPrefs.SetInt("viewersLevel", 0);
    }
}
