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
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
           
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if(skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
        int yDir = UnityEngine.Random.value < 0.5f ? -1 : 1;

        AttemptMove<Component>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        
    }
}
