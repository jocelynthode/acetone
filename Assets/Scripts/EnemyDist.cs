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
        RaycastHit2D hit;

        if ((Math.Abs(player.transform.position.x - transform.position.x) > float.Epsilon) &&
            (Math.Abs(player.transform.position.y - transform.position.y) > float.Epsilon))
        {
            base.Move();
        }
        else
        {
            boxCollider.enabled = false;
            if (Math.Abs(player.transform.position.x - transform.position.x) < float.Epsilon)
            {
                if (player.transform.position.y > transform.position.y)
                    hit = Physics2D.Raycast(transform.position, transform.up, Mathf.Infinity, blockingLayer);
                else
                    hit = Physics2D.Raycast(transform.position, -transform.up, Mathf.Infinity, blockingLayer);
                if (hit.collider.gameObject.tag == "Player")
                    Attack(player);
                else
                    base.Move();
            }
            else
            {
                if (player.transform.position.x > transform.position.x)
                    hit = Physics2D.Raycast(transform.position, transform.right, blockingLayer);
                else
                    hit = Physics2D.Raycast(transform.position, -transform.right, blockingLayer);
                print(hit.collider.gameObject.tag);
                if (hit.collider.gameObject.tag == "Player")
                    Attack(player);
                else
                    base.Move();       
            }
            boxCollider.enabled = true;
        }
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
        animator.SetTrigger("EnemyAttack");
    }

    private void DisplayLaser(Vector3 own, Vector3 player) {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, own);
        lineRenderer.SetPosition(1, player);
    }
}
