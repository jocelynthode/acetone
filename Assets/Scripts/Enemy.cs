using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MovingObject
{

    protected Animator anAnimator;
    private Transform target;
    private bool skipMove;
    private int dir = 1;
    protected int startHp;
    public bool isSmart;

    public virtual float KillMoneyCoef {
        get {
            return isSmart ? 4 : 1;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        anAnimator = GetComponent<Animator>();
        base.Start();
        hp = startHp = (int) ( 60 * (1 + GameManager.instance.level / 10f) );
        att = (int) ( 5 * (1 + GameManager.instance.level / 20f) );
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
        if (isSmart)
        {
            AStar aStar = GameManager.instance.aStar;
            ArrayList a = aStar.calculatePath(this.transform.position, player.position);
            Vector3 nextPosition = (Vector3) a[0] - transform.position;
            xDir = (int) nextPosition.x;
            yDir = (int) nextPosition.y;
        }
        else
        {
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
        Utils.PlaySound("ennemyHit");
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
        int moneyGainCoef = PlayerPrefs.GetInt("moneyGain");
        GameManager.instance.moneyGain += (int) (startHp * 0.2f * (moneyGainCoef / 100f) * KillMoneyCoef);
        Player.instance.RefreshUI();
        Destroy(gameObject);
    }

}
