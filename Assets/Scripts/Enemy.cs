using UnityEngine;
using System;
using UnityEngine.UI;

public class Enemy : MovingObject
{

    protected Animator anAnimator;
    private Transform target;
    private bool skipMove;
    private int dir = 1;
    public AudioSource hit;
    
    // Use this for initialization
    protected override void Start()
    {
        anAnimator = GetComponent<Animator>();
        base.Start();
        hp = 100;
        att = 1;
        def = 1;
    }

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
    }


    public virtual void Move()
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
        if (xDir != 0 && xDir != dir)
        {
            Flip();
            dir = xDir;
        }
        AttemptMove<Player>(xDir, yDir);
    }

    public override void TakeDamage(int att)
    {
        base.TakeDamage(att);
        hit.Play();
        anAnimator.SetTrigger("EnemyHit");
    }

    void Flip()
    {
        this.anAnimator.transform.Rotate(0, 180, 0);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player player = component as Player;
        player.TakeDamage(att);
        anAnimator.SetTrigger("EnemyAttack");
        GameManager.instance.waitOnEnemiesAnimations = true;
    }

    public override void Die()
    {
        GameManager.instance.boardScript.enemies.Remove(gameObject.GetComponent<Enemy>());
        GameManager.instance.moneyGain += 15;
        GameManager.instance.moneyGainText = GameObject.Find("moneyGainText").GetComponent<Text>();
        GameManager.instance.moneyGainText.text = string.Format("SKILL MONEY: ${0}", GameManager.instance.moneyGain);
        Destroy(gameObject);
    }

}
