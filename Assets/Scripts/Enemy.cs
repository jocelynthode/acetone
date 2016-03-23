using UnityEngine;
using System.Collections;
using Completed;
using System;

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

    protected override void OnCantMove<T>(T component)
    {
        Player player = component as Player;
        player.takeDamage(1);
        animator.SetTrigger("EnemyAttack");
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

}
