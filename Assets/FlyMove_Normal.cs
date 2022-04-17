﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class FlyMove_Normal : EnemyMoveBase
{
    private float moveSpeed = 0f;

    private Vector3 moveDir;

    [SerializeField]
    protected Rigidbody2D rb;

    private bool initialized = false;

    [SerializeField]
    private AgentHpController agentHpController;

    private bool isDamaged = false;

    private Transform playerTr;

    [SerializeField]
    private Transform viewTr;

    [SerializeField]
    private Collider2D collider;

    [SerializeField]
    private bool isDownOnly = false;

    public void Initialize(Vector3 moveDir, float moveSpeed)
    {
        isDamaged = false;

        SetMoveDir(moveDir, moveSpeed);

        if (initialized == false)
        {
            initialized = true;
            Subscribe();
        }

        collider.isTrigger = false;
    }

    private void Subscribe()
    {
        playerTr = PlayerMoveController.Instance.transform;

        agentHpController.whenEnemyDamaged.AsObservable().Subscribe(e =>
        {
            if (isDownOnly == false)
            {
                isDamaged = true;
                collider.isTrigger = true;
            }


        }).AddTo(this);
    }

    private void Update()
    {
        if (isDamaged == false)
        {
            rb.velocity = moveDir.normalized * moveSpeed;

        }
        else
        {
            if (isDownOnly == false)
            {
                float playerDist = Vector3.Distance(playerTr.position, this.transform.position);

                if (playerDist >= 0.1f)
                {
                    Vector3 moveDir = playerTr.position - this.transform.position;
                    rb.velocity = moveDir.normalized * moveSpeed * 1.5f;
                }
            }
        }


        if (isDownOnly == false)
        {
            viewTr.transform.localScale = new Vector3(rb.velocity.x > 0 ? -1 : 1, 1, 1);
        }
    }

    private void SetMoveDir(Vector3 moveDir, float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.moveDir = moveDir;

        if (isDownOnly)
        {
            this.moveDir = Vector3.down;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDamaged == true) return;

        //Vector3 refrectDir = this.transform.position - (Vector3)collision.GetContact(0).point;

        if (isDownOnly == false)
        {
            SetMoveDir(Quaternion.Euler(0f, 0f, Random.Range(200, 340)) * moveDir, this.moveSpeed);
        }
        //SetMoveDir(refrectDir, this.moveSpeed);
    }
}
