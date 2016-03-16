using UnityEngine;
using System.Collections;
using Completed;
using System;

public class Player : MovingObject{

    private Animator animator;


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

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0) vertical = 0;
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Enemy>(horizontal, vertical);
	}


    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
        base.turn = false;

    }

    protected override void OnCantMove<T>(T component)
    {
        base.turn = true;
    }
}
