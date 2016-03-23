using UnityEngine;
using System.Collections;
using Completed;
using System;

public class Player : MovingObject{

    private Animator animator;
    private int dir = 1;


    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();
        base.Start();
	}

    private void Update()
    {
        if (!GameManager.instance.playerTurn) return;
        int horizontal = 0;
        int vertical = 0;
        int fire1 = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        fire1 = (int)Input.GetAxisRaw("Fire1");

        if (fire1 > 0)
        {
            animator.SetTrigger("PlayerAtt");
            attack();
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

    private void attack()
    {

    }

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
        GameManager.instance.playerTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
    }
    void Flip()
    {
        animator.transform.Rotate(0, 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Exit")
        {
            GameManager.instance.OnLevelCompletion();
        }
    }
}
