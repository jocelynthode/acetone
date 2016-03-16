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
	}

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
    }

    public void Move()
    {
        int xDir = 0;
        int yDir = 0;
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.5f)
        {
            xDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
        }
        else
        {
            yDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
        }
        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
    }
}
