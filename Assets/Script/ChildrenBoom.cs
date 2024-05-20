﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChildrenBoom : MonoBehaviour
{
    [SerializeField]
    GameObject imgBombPrefab;
    [SerializeField]
    GameObject exploderPrefab;
    Rigidbody2D rb;

    [SerializeField]
    float timeDisActive;

    float timer;

    private void Awake()
    {
        exploderPrefab.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        timer = timeDisActive;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - transform.position).normalized;
        rb.AddForce(direction * 800f, (ForceMode2D)ForceMode.Force);
    }

    private void Update()
    {
        Invoke("StopMove", 0.3f);
        timer -= Time.deltaTime;
        if (timer < 0 )
        {
            timer = timeDisActive;
            DisActivateGameObject();
        }
    }

    public void StopMove()
    {
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemy=collision.GetComponent<EnemyBase>();
        if(enemy != null)
        {
            imgBombPrefab.SetActive(false);
            exploderPrefab.SetActive(true);
        }
    }

    private void DisActivateGameObject()
    {
        imgBombPrefab.SetActive(false);
        exploderPrefab.SetActive(true);
        Destroy(gameObject,0.4f);
    }
}