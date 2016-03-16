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
	
	// Update is called once per frame
	void Update () {
        if (!base.turn) return;
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


    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.turn = false;
        base.AttemptMove<T>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        base.turn = true;
    }
    void Flip()
    {
        animator.transform.Rotate(0, 180, 0);
    }
}
