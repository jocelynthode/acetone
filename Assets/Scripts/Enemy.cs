using UnityEngine;
using System;
using UnityEngine.UI;

public class Enemy : MovingObject {

    private Animator animator;
    private Transform target;
    private bool skipMove;

    // Use this for initialization
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        base.Start();
        hp = 100;
        att = 1;
        def = 1;

    }

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
    }


    public void Move()
    {
        Transform player = GameManager.instance.boardScript.player.transform;
        int xDir = 0;
        int yDir = 0;
        float value = UnityEngine.Random.value;
        //TODO make the enemy choose something else if there is obstacle (RayCast)
        if (value > 0.1f)
        {
            if (Math.Abs(player.position.x - this.transform.position.x) < float.Epsilon)
            {
                yDir = player.position.y > this.transform.position.y ? 1 : -1;
            }
            else
            {
                xDir = player.position.x > this.transform.position.x ? 1 : -1;
            }
        }
        AttemptMove<Player>(xDir, yDir);
    }

	public override void TakeDamage(int att)
	{
		base.TakeDamage(att);
		animator.SetTrigger("EnemyHit");
	}

    protected override void OnCantMove<T>(T component)
    {
        Player player = component as Player;
        player.TakeDamage(att);
        animator.SetTrigger("EnemyAttack");
    }

    protected override void Die()
    {
        GameManager.instance.boardScript.enemies.Remove(gameObject.GetComponent<Enemy>());
		GameManager.instance.moneyGain += 15;
		GameManager.instance.moneyGainText = GameObject.Find ("moneyGainText").GetComponent<Text> ();
		GameManager.instance.moneyGainText.text = string.Format("SKILL MONEY: {0:C0}", GameManager.instance.moneyGain);
        Destroy(gameObject);
    }

}
