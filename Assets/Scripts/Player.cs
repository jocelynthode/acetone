using UnityEngine;
using UnityEngine.UI;
using System;
using GameState = GameManager.GameState;

public class Player : MovingObject{

    private Animator animator;
    private int dir = 1;
    public Text healthPoint;
    public Text money;
	public Text viewers;
    public static Player instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            animator = GetComponent<Animator>();
        } else if (instance != this)
        {   
            Destroy(gameObject);
            instance.InitLevel();
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        hp = PlayerPrefs.GetInt("maxHealth");
        att = PlayerPrefs.GetInt("attack");
        def = PlayerPrefs.GetInt("defense");
        InitLevel();
	}

    private void InitLevel()
    {
        healthPoint = GameObject.Find("hpText").GetComponent<Text>();
        healthPoint.text = "HP: " + hp.ToString();
        money = GameObject.Find("moneyText").GetComponent<Text>();
        money.text = string.Format("Money: {0:C2}", PlayerPrefs.GetInt("money", 0));
		viewers = GameObject.Find ("viewersText").GetComponent<Text>();
		viewers.supportRichText = true;
		viewers.text = string.Format("<color=red><size=16> • </size></color> {0}", PlayerPrefs.GetInt("viewers", 0));
        transform.position = new Vector2(0, 0);
    }

    private void Update()
    {
        if (GameManager.instance.state != GameState.PLAYERTURN) return;
        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Alpha1)){
            instance.Die();
            return;
        }

        if (Input.GetKey(KeyCode.Alpha2)){
            if (GameManager.instance.state != GameState.PLAYERTURN) return;
            GameManager.instance.OnTurnEnd();
            Items.useItem(Items.ItemType.BOMB);
            
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

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
        GameManager.instance.OnTurnEnd();
    }

    protected override void OnCantMove<T>(T component)
    {
        Enemy enemy = component as Enemy;
        enemy.TakeDamage(att);
        animator.SetTrigger("PlayerAtt");
    }

    public override void TakeDamage(int att)
    {
        base.TakeDamage(att);
        healthPoint.text = "HP: " + hp.ToString();
        animator.SetTrigger("PlayerHit");
    }

    void Flip()
    {
        animator.transform.Rotate(0, 180, 0);
    }

    public override void Die()
    {
        GameManager.instance.OnGameOver();
    }

    public int getHP()
    {
        return base.hp;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Exit")
        {
            GameManager.instance.OnLevelCompletion();
        }
    }
}
