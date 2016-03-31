﻿using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MovingObject{

    private Animator animator;
    private int dir = 1;
    public Text healthPoint;


    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();
        base.Start();
        hp = 100;
        att = 10;
        def = 1;
        PlayerPrefs.SetInt("money", 100);
        PlayerPrefs.SetInt("viewers", 1000);
        healthPoint.text ="HP: "+hp.ToString();
	}


    private void Update()
    {

        if (!GameManager.instance.playerTurn) return;
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

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

    public override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
        GameManager.instance.playerTurn = false;
        GameManager.instance.OnTurnEnd();
    }

    protected override void OnCantMove<T>(T component)
    {
        Enemy enemy = component as Enemy;
        enemy.TakeDamage(att);
        animator.SetTrigger("PlayerAtt");
    }

    public override void TakeDamage(int att)
    {

        base.TakeDamage(att);
        healthPoint.text = "HP: " + hp.ToString();
        animator.SetTrigger("PlayerHit");

    }

    void Flip()
    {
        animator.transform.Rotate(0, 180, 0);
    }

    protected override void Die()
    {
        GameManager.instance.OnGameOver();
    }

    public int getHP()
    {
        return base.hp;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Exit")
        {
            GameManager.instance.OnLevelCompletion();
        }
    }
}
