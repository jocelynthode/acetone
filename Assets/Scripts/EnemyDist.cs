using UnityEngine;
using System;
using UnityEngine.UI;

public class EnemyDist : Enemy {

    private Transform target;
    private bool skipMove;
    public GameObject laser;


    public override void AttemptMove<T>(int xDir, int yDir)
    {
        RaycastHit2D hit;
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);

        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            rb2D.MovePosition(end);
        }
    }


    public void Move()
    {
        Transform player = GameManager.instance.boardScript.player.transform;
        int xDir = 0;
        int yDir = 0;
        RaycastHit2D hit;
        bool willAttack = false;
        Attack(Player);
        /*
        if (Math.Abs(player.position.x - transform.position.x) < float.Epsilon)
        {
            hit = Physics2D.Linecast(transform.position, player.position, blockingLayer);
            if (hit.collider.gameObject.tag == "Player")
            {
                willAttack = true;
            }
            yDir = player.position.y > this.transform.position.y ? 1 : -1;
        }
        else if (Math.Abs(player.position.y - transform.position.y) < float.Epsilon)
        {
            hit = Physics2D.Linecast(transform.position, player.position, blockingLayer);
            if (hit.collider.gameObject.tag == "Player")
            {
                willAttack = true;
            }
            xDir = player.position.x > this.transform.position.x ? 1 : -1;
        }
        else
        {
            yDir = player.position.y > this.transform.position.y ? 1 : -1; 
        }
        if (willAttack)
        {
            Attack(player);
        }
        else
        {
            AttemptMove<Player>(xDir, yDir);
        }
        */
    }

	public override void TakeDamage(int att)
	{
		base.TakeDamage(att);
		animator.SetTrigger("EnemyHit");
	}

    private void Attack<T>(T component)
    {
        Player player = component as Player;

        Laser laser = Instantiate(laser, transform.position, Quaternion.identity);
        laser.DisplayLaser(this.transform, player.transform);
        player.TakeDamage(att);
        animator.SetTrigger("EnemyAttack"); //TODO review Triggers
    }
}
