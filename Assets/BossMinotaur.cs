﻿using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMinotaur : EnemyBase
{
    [SerializeField]
    //Nhận biết tấn công player 
    public GameObject targetGameObject;

    [SerializeField]
    GameObject slashSkillPrefab;
    [SerializeField] Transform Skill1Pos;

    [SerializeField]
    GameObject Skill1;

    [SerializeField] List<GameObject> Skill2Lst = new List<GameObject>();



    float timer;
    [SerializeField]
    float timeSkill1 = 6;
    [SerializeField]
    float timeSkill2 = 9;

    float timerSkill1;
    float timerSkill2;

    Animator animator;

    [SerializeField]
    GameObject HealthPrefab;
    [SerializeField]
    GameObject ChestPrefab;
    [SerializeField]
    GameObject ExpGreenPrefab;
    [SerializeField]
    GameObject ExpRedPrefab;
    [SerializeField]
    GameObject CoinPrefab;

    [SerializeField]
    int numberDropCoins;
    [SerializeField]
    int numberDropExp;

    GameObject ParentDropItem;

    bool isUseSkill1=false;


    private void Start()
    {
        animator = GetComponent<Animator>();
        timerSkill1 = timeSkill1;
        timerSkill2 = timeSkill2;
    }

    private void Update()
    {
        GetComponent<SpriteRenderer>().flipX = transform.position.x > targetGameObject.transform.position.x;

        timer -=Time.deltaTime;
        timerSkill1 -= Time.deltaTime;
        timerSkill2 -= Time.deltaTime;

        if( timerSkill1 <= 0)
        {
            timerSkill1 = timeSkill1;
            StartCoroutine(SkillOne());
        }
        else if( timerSkill2 <= 0&& !isUseSkill1) 
        {
            timerSkill2 = timeSkill2;
            SkillTwo();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CharacterInfo_1 player=collision.GetComponent<CharacterInfo_1>();

        if (timer <= 0&& player!=null)
        {
            timer = enemyStats.timeAttack;
            player.TakeDamage(enemyStats.dmg);
        }
    }

    private IEnumerator SkillOne()
    {
        animator.SetBool("Skill1", true);
        isUseSkill1=true;
        yield return new WaitForSeconds(0.02f);
        Vector2 lookDir = targetGameObject.transform.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        Skill1.transform.rotation = Quaternion.Euler(0, 0, angle);

        GameObject slash= Instantiate(slashSkillPrefab);
        slash.GetComponent<SlashSkill1MinotairBoss>().SetTargetAndDmg(targetGameObject,enemyStats.dmg);
        slash.transform.position = Skill1Pos.position;
        slash.transform.rotation=Skill1.transform.rotation;
        Rigidbody2D rb= slash.GetComponent<Rigidbody2D>();
        rb.AddForce(slash.transform.right * 9f, ForceMode2D.Impulse);
        Destroy(slash, 5f);
        yield return new WaitForSeconds(0.03f);
        animator.SetBool("Skill1", false);
        isUseSkill1 = false;
    }

    private void SkillTwo()
    {

    }

    public override void SetTarget(GameObject GameObject)
    {
        targetGameObject = GameObject;
        gameObject.GetComponent<AIPath>().maxSpeed = enemyStats.speed;
        SetTargetSkill2();
        GetComponent<AIDestinationSetter>().SetTarget(targetGameObject);
    }

    private void SetTargetSkill2()
    {
        foreach (var item in Skill2Lst)
        {
            item.GetComponent<RockFallingMap3>().SetPlayerTarget(targetGameObject);
        }
    }

    public override void SetParentDropItem(GameObject gameObject)
    {
        ParentDropItem = gameObject;
    }

    public override bool EnemyTakeDmg(int dmg)
    {
        enemyStats.hp -= dmg;
        animator.SetTrigger("Hit");
        if (enemyStats.hp <= 0)
        {
            GetComponent<AIPath>().canMove = false;
            GetComponent<Rigidbody2D>().simulated = false;
            animator.SetBool("Dead", true);
            Destroy(gameObject, 1f);
            Drop();
            return true;
        }
        return false;
    }

    private void Drop()
    {
        Transform health = Instantiate(HealthPrefab).transform;
        health.position = RandomPositionDrops(transform.position);
        health.transform.parent = ParentDropItem.transform;

        Transform chest = Instantiate(ChestPrefab).transform;
        chest.position = RandomPositionDrops(transform.position);

        for (int i = 0; i <= numberDropCoins; i++)
        {
            GameObject createCoins = Instantiate(CoinPrefab);
            createCoins.transform.position = RandomPositionDrops(transform.position);
            createCoins.GetComponent<CoinScript>().SetPlayer(targetGameObject);
            createCoins.transform.parent = ParentDropItem.transform;
        }
        for (int i = 0; i <= numberDropExp; i++)
        {
            GameObject createExpRed = Instantiate(ExpRedPrefab);
            createExpRed.transform.position = RandomPositionDrops(transform.position);
            createExpRed.GetComponent<CapsuleExp>().SetPlayer(targetGameObject);
            createExpRed.transform.parent = ParentDropItem.transform;
        }
    }

    public Vector3 RandomPositionDrops(Vector3 center)
    {
        float radius = 5f;
        // Tạo một vector direction random
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        // Thêm vector direction vào vị trí trung tâm
        Vector3 randomPosition = center + randomDirection;

        return randomPosition;
    }
}
