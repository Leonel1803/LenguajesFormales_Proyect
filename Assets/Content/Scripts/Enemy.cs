using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    private float health, maxHealth = 100f;

    private float moveSpeed = 2f;
    [SerializeField] FloatingHealthBar healthBar;
    Rigidbody enemyRb;
    GameObject player;
    Transform target;
    Animator enemyAnim;
    NavMeshAgent navMeshAgent;
    public Collider teletubbieHand;

    void Start()
    {
        health = maxHealth;
        player = GameObject.Find("Player");
        target = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.SetDestination(target.position);

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                teletubbieHand.isTrigger = true;
                enemyAnim.Play("Hit");
                StartCoroutine(DisableColliderAfterAnimation());
            }
            else
            {
                RunningAnimLogic();
            }
        }
    }

    private void Awake()
    {
        enemyRb = this.GetComponent<Rigidbody>();
        enemyRb.isKinematic = true;
        enemyAnim = GetComponentInChildren<Animator>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    public void RunningAnimLogic()
    {
        enemyAnim.SetFloat("X", target.position.x);
        enemyAnim.SetFloat("Y", target.position.y);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LeftHand" || other.gameObject.tag == "RightHand")
        {
            health -= 10f;
            TakeDamage();
        }
        else if(other.gameObject.tag == "RightElbow")
        {
            health -= 15f;
            TakeDamage();
        }
        else if (other.gameObject.tag == "Foot")
        {
            health -= 20f;
            TakeDamage();
        }
        else if (other.gameObject.tag == "PistolDamage")
        {
            health -= 35f;
            TakeDamage();
        }
        else if (other.gameObject.tag == "RifleDamage")
        {
            health -= 50f;
            TakeDamage();
        }

        if (health <= 0)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
            Die();
        }
    }

    public void TakeDamage()
    {
        healthBar.UpdateHealthBar(health, maxHealth);
        enemyAnim.Play("Hitted");
        navMeshAgent.enabled = false;

        StartCoroutine(EnableNavMeshAfterAnimation("Hitted"));
    }

    void Die()
    {
        navMeshAgent.enabled = false;
        enemyAnim.StopPlayback();
        enemyAnim.SetBool("dead", true);
        Destroy(gameObject, 4f);
    }

    IEnumerator EnableNavMeshAfterAnimation(string animationName)
    {
        yield return new WaitForSeconds(enemyAnim.GetCurrentAnimatorStateInfo(0).length);

        navMeshAgent.enabled = true;
    }

    IEnumerator DisableColliderAfterAnimation()
    {
        yield return new WaitForSeconds(enemyAnim.GetCurrentAnimatorStateInfo(0).length - 2.5f);

        teletubbieHand.isTrigger = false;
    }
}
