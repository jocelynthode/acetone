using UnityEngine;
using System.Collections;

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
            int fire1 = 0;

            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");
            fire1 = (int)Input.GetAxisRaw("Fire1");

            if (fire1 > 0)
            {
                animator.SetTrigger("PlayerAtt");
                return;
            }


            if (horizontal != 0)
            {
                vertical = 0;
                if (horizontal != dir)
                {
                    Flip();
                    dir = horizontal;
                }
            }
            if (horizontal != 0 || vertical != 0)
                AttemptMove<Enemy>(horizontal, vertical);

        }
        else
        {
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
            AttemptMove<Player>(xDir, yDir);
        }
	}
}
