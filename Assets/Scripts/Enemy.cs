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

    protected override void OnCantMove<T>(T component)
    {
    }
}
