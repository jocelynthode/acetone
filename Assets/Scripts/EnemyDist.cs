using UnityEngine;
using System;
using UnityEngine.UI;

public class EnemyDist : Enemy {
    private LineRenderer lineRenderer;

    protected override void Start () {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

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


    public override void Move()
    {
        Player player = GameManager.instance.boardScript.player;
        int xDir = 0;
        int yDir = 0;
        RaycastHit2D hit;
        Attack(player);
        //TODO Either move or attack depending if we have line of sight or not
    }

	public override void TakeDamage(int att)
	{
		base.TakeDamage(att);
		animator.SetTrigger("EnemyHit");
	}

    private void Attack(Player player)
    {
        DisplayLaser(transform.position, player.transform.position);
        player.TakeDamage(att);
        animator.SetTrigger("EnemyAttack"); //TODO review Triggers
    }

    private void DisplayLaser(Vector3 own, Vector3 player) {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, own);
        lineRenderer.SetPosition(1, player);
    }
}
