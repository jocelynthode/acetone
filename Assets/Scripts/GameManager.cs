using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public bool playerTurn = true;
    private bool enemiesMoving = false;
    public int level;

    private IEnumerator enemiesCoroutine;

    public BoardManager boardScript;

	// Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        playerTurn = true;
        enemiesMoving = false;
        boardScript.SetupScene(level);
    }
	
	// Update is called once per frame
	void Update () {
        if (playerTurn || enemiesMoving) return;
        enemiesCoroutine = MoveEnemies();
        StartCoroutine(enemiesCoroutine);
	}

    public void OnLevelCompletion()
    {
        StopCoroutine(enemiesCoroutine);
        // Show upgrade screen
        level++;
        // InitGame();
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
