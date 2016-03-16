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

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        int fire1 = 0;

        fire1 = (int)Input.GetAxisRaw("Fire1");

        if (fire1 > 0)
        {
            animator.SetTrigger("PlayerAtt");
            return;
        }


        if (xDir != 0)
        {
            yDir = 0;
            if (xDir != dir)
            {
                Flip();
                dir = xDir;
            }
        }
        if (xDir != 0 || yDir != 0)
            base.AttemptMove<T>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
    }
    void Flip()
    {
        animator.transform.Rotate(0, 180, 0);
    }
}
