﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    Vector2 playerLoc;
    private NavMeshAgent nav;
    private GameObject player;
    public Drops drops;
    private Renderer rend;
    private new AudioSource audio;
    new Rigidbody2D rigidbody2D;
    Animator animator;
    float timer;
    float resetTime = .5f;
    private float distanceToPlayer;
    Vector2 lookDirection = new Vector2(1, 0);

    private float health = 15;
    private bool damaged = false;
    public bool dead = false;
    //Color lerp stuff 
    Color colorStart = Color.red;
    Color colorEnd = Color.white;
    //---
    bool dropShotgun = false;
    bool dropMagnum = false;
    bool dropBouncer = false;
    bool dropLauncher = false;
    void Start()
    {
        drops = GetComponent<Drops>();
        nav = GetComponent<NavMeshAgent>();
        audio = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        nav.updateRotation = false;
        nav.updateUpAxis = false;
        player = GameObject.FindGameObjectWithTag("Player");
        playerLoc = player.transform.position;
        timer = 1.0f;
        rigidbody2D.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    private void Awake()
    {
        health += LevelManager.difficulty;
        float roll = GetComponent<Drops>().DropRoll();

        if (roll < 25)
        {
            dropShotgun = true;
            return;
        }
        if (roll > 25 && roll < 50)
        {
            dropBouncer = true;
            return;
        }
        if (roll > 50 && roll < 70)
        {
            dropMagnum = true;
            return;
        }
        if (roll > 70 && roll < 100)
        {
            dropLauncher = true;
            return;
        }
    }

    void Update()
    {
        if (dead)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (this.health <= 0)
        {
            Kill();
        }

        lookDirection = player.gameObject.transform.position - transform.position;
        lookDirection.Normalize();
        lookDirection /= 2;

        playerLoc = player.transform.position;
        distanceToPlayer = Vector2.Distance(transform.position, playerLoc);
        if (health < 15)
        {
            MoveToPlayer();
        }
        if (distanceToPlayer < 4f)
        {
            if (timer < 0)
            {
                
                timer = resetTime;

                animator.SetFloat("Move X", lookDirection.x);
                animator.SetFloat("Move Y", lookDirection.y);
                Attack();
            }
        }
        if (distanceToPlayer > 3.5f && distanceToPlayer < 11.5)
        {
            animator.SetBool("AttackLeft", false);
            animator.SetBool("AttackRight", false);
            animator.SetBool("AttackUp", false);
            animator.SetBool("AttackDown", false);

            animator.SetFloat("Move X", lookDirection.x);

            animator.SetFloat("Move Y", lookDirection.y);

            MoveToPlayer();
        }

        if (LevelManager.remainingEnemies < 4)
        {
            MoveToPlayer();
        }

    }

    float CurveWeightedRandom(AnimationCurve curve)
    {
        return curve.Evaluate(Random.value) * 100;
    }

    private void MoveToPlayer()
    {
        nav.SetDestination(playerLoc);
    }

    public void Damage(float dmgValue)
    {
        if (!damaged)
        {
            damaged = true;
            StartCoroutine("SwitchColor");
        }
        health -= dmgValue;
    }

    IEnumerator SwitchColor()
    {
        rend.material.color = new Color(1f, 0.30196078f, 0.30196078f);
        yield return new WaitForSeconds(.2f);
        rend.material.color = Color.white;
        damaged = false;
    }

    private void Kill()
    {
        nav.speed = 0;
        dead = true;
        this.GetComponentInChildren<Hitbox>().gameObject.layer = 16;
        animator.SetTrigger("Dead");
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        DudeController playerController = player.gameObject.GetComponent<DudeController>();
        playerController.ChangeMoney(1);
        playerController.ChangeXp(11);
        LevelManager.remainingEnemies--;

        if (dropShotgun)
        {
            drops.DropShotgunAmmo(this.gameObject.transform.position);
        }
        if (dropMagnum)
        {
            drops.DropMagnumAmmo(this.gameObject.transform.position);
        }
        if (dropBouncer)
        {
            drops.DropBouncerAmmo(this.gameObject.transform.position);
        }
        if (dropLauncher)
        {
            drops.DropLauncherAmmo(this.gameObject.transform.position);
        }

        if (LevelManager.remainingEnemies <= 0)
        {
            LevelManager.WinCheck();
        }
    }

    void Attack()
    {
        if (distanceToPlayer < 4.5f)
        {
            if (lookDirection.x < -.4f)
            {
                animator.SetBool("AttackLeft", true);
                nav.speed = 4;
            } 

            if (lookDirection.x > .4f)
            {
                animator.SetBool("AttackRight", true);
                nav.speed = 4;
            }

            if (lookDirection.y > .4f)
            {
                animator.SetBool("AttackUp", true);
                nav.speed = 4;
            }
            if (lookDirection.y < -.4f)
            {
                animator.SetBool("AttackDown", true);
                nav.speed = 4;
            }
        }
        StartCoroutine(Freeze());
    }

    // Called from Boss animations
    void DamagePlayer()
    {
        if (distanceToPlayer < 3.4)
        {
            player.gameObject.GetComponent<DudeController>().ChangeHealth(-2);
        }
    }

    IEnumerator Freeze()
    {
        yield return new WaitForSeconds(1);
        nav.speed = 5.2f;
    }
}
