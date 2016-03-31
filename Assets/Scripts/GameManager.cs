using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager {
    private static GameManager instance = new GameManager();
    public static GameManager Instance
    {
        get { return instance; }
    }

    public bool playerTurn;
    private bool enemiesMoving;
    public int level;
    public bool levelSetup = true;  // Probably useless
    private GameObject scene;

    private IEnumerator enemiesCoroutine;

    public BoardManager boardScript;

    private GameManager()
    {
        SceneManager.LoadScene("LevelProcedural");
        //SceneManager.LoadScene("UpgradeMenu");
        scene = GameObject.Find("Scenee");
        boardScript = scene.GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        //boardScript = GameObject.GetComponent<BoardManager>();
        playerTurn = true;
        enemiesMoving = false;
        boardScript.SetupScene(3);
        levelSetup = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (playerTurn || enemiesMoving || levelSetup) return;
        enemiesCoroutine = MoveEnemies();
        boardScript.StartCoroutine(enemiesCoroutine);
	}

    public void OnLevelCompletion()
    {
        levelSetup = true;
        boardScript.StopCoroutine(enemiesCoroutine);
        // Show upgrade screen
        level++;
        // InitGame();
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnGameOver()
    {
        boardScript.StopCoroutine(enemiesCoroutine);
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
}
