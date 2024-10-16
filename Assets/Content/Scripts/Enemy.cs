using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static event Action<Enemy> OnEnemyKilled;
    [SerializeField] float health, maxHealth = 10f;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] FloatingHealthBar healthBar;
    Rigidbody rb;
    Transform target;
   
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        health = maxHealth;
        target = GameObject.Find("Player").transform;
    }
    void Update()
    {
        if (target && (target.position - rb.transform.position).magnitude < 10)
        {
            rb.transform.position = Vector3.MoveTowards(rb.transform.position, target.position, this.moveSpeed * Time.deltaTime);
        }
    }
}
