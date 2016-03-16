using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public bool playerTurn = true;

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
        boardScript.SetupScene(3);
    }
	
	// Update is called once per frame
	void Update () {
        if (playerTurn)
        {
            int horizontal = 0;
            int vertical = 0;

            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");

            boardScript.player.AttemptMove<Enemy>(horizontal, vertical);
            playerTurn = false;
        }
        else
        {
            boardScript.enemies.ForEach(moveEnemies);
            playerTurn = true;
        }
	}

    private void moveEnemies(UnityEngine.Object obj)
    {
        Enemy enemy = (Enemy)obj;
        int xDir = 0;
        int yDir = 0;
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.5f)
        {
            xDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
        }
        else
        {
            yDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
        }
        enemy.AttemptMove<Player>(xDir, yDir);
    }
}
