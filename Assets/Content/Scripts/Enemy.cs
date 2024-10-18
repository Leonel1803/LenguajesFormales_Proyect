using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    private float health, maxHealth = 100f;

    private float moveSpeed = 1f;
    private float chaseSpeed = 4f;
    [SerializeField] FloatingHealthBar healthBar;
    Rigidbody enemyRb;
    GameObject player;
    Transform target;
    Animator enemyAnim;
    NavMeshAgent navMeshAgent;
    public Collider teletubbieHand;
    public GameObject playerObject;
    PlayerController playerController;

    public float wanderRange = 5f;
    public float detectionRadius = 10f;
    private Vector3 wanderPointA;
    private Vector3 wanderPointB;
    private Vector3 targetWanderPoint;
    private bool isChasing = false;
    private bool movingRight = true;

    void Start()
    {
        health = maxHealth;
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        UnityEngine.Debug.Log(playerController);
        target = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        wanderPointA = transform.position - Vector3.right * wanderRange;
        wanderPointB = transform.position + Vector3.right * wanderRange;
        targetWanderPoint = wanderPointB;
        navMeshAgent.SetDestination(targetWanderPoint);
    }

    void Update()
    {

        
        if (playerController.health <= 0)
        {
            navMeshAgent.enabled = false;
            Dance();
        }
        else if (navMeshAgent.enabled)
        {
            //navMeshAgent.speed = moveSpeed;
            //navMeshAgent.SetDestination(target.position);

            //if (navMeshAgent.remainingDistance <= detectionRadius)

            float playerDistance = Vector3.Distance(transform.position, player.transform.position);
            //UnityEngine.Debug.Log(playerDistance);

            if (playerDistance <= detectionRadius)
            {
                isChasing = true;
                ChasePlayer();
            }
            else
            {
                isChasing = false;
                Wander();
            }


            UpdateMovementAnimation();
        }
    }

    void ChasePlayer()
    {
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(target.position);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            teletubbieHand.isTrigger = true;
            enemyAnim.Play("Hit");
            StartCoroutine(DisableColliderAfterAnimation());
        }
        else
        {
            enemyAnim.SetBool("isWalking", false);
            enemyAnim.SetBool("isRunning", true);
        }
    }

    void UpdateMovementAnimation()
    {
        Vector3 velocity = navMeshAgent.velocity;

        if (velocity.magnitude > 0.1f)
        {
            if (isChasing)
            {
                enemyAnim.SetBool("isRunning", true);
                enemyAnim.SetBool("isWalking", false);
            }
            else
            {
                enemyAnim.SetBool("isWalking", true);
                enemyAnim.SetBool("isRunning", false);
            }
        }
        else
        {
            enemyAnim.SetBool("isWalking", false);
            enemyAnim.SetBool("isRunning", false);
        }
    }

    void Wander()
    {
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.SetDestination(targetWanderPoint);

        if (Vector3.Distance(transform.position, targetWanderPoint) < 1f)
        {
            if (movingRight)
            {
                targetWanderPoint = wanderPointA;
            }
            else
            {
                targetWanderPoint = wanderPointB;
            }
            movingRight = !movingRight;
        }

        enemyAnim.SetBool("isWalking", true);
        enemyAnim.SetBool("isRunning", false);
    }

    private void Dance()
    {
        enemyAnim.SetBool("dance", true);
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
        else if (other.gameObject.tag == "RightElbow")
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
            Destroy(other.gameObject);
            navMeshAgent.enabled = false;
            health -= 35f;
            TakeDamage();
        }
        else if (other.gameObject.tag == "RifleDamage")
        {

            Destroy(other.gameObject);
            navMeshAgent.enabled = false;
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
        navMeshAgent.enabled = false;
        healthBar.UpdateHealthBar(health, maxHealth);
        enemyAnim.Play("Hitted");

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
