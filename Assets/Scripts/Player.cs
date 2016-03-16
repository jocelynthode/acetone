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

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
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
