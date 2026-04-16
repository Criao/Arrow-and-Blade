using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float playerDetectRange = 5f;
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer; // 障碍物图层（高台、墙壁等）
    private float attackCoolDownTime;
    private EnemyState enemyState;

    private Transform player;
    private int facingDirection = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }
    private void Update()
    {
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();
            if (attackCoolDownTime > 0)
            {
                attackCoolDownTime -= Time.deltaTime;
            }
            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                rb.velocity = Vector2.zero;
            }
        }


    }
    private void Chase()
    {

        if ((player.position.x > transform.position.x && facingDirection == -1) || (player.position.x < transform.position.x && facingDirection == 1))
        {
            Filp();
        }
        Vector2 direction = (player.transform.position - transform.position).normalized;//(normalized归一化)
        rb.velocity = direction * speed;
    }
    private void Filp()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;

            // 射线检测：检查敌人和玩家之间是否有障碍物
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

            // 如果射线碰到障碍物，说明玩家被遮挡，敌人无法追踪
            if (hit.collider != null)
            {
                rb.velocity = Vector2.zero;
                ChangeState(EnemyState.Idle);
                return;
            }

            //如果玩家在攻击范围内并且攻击冷却时间小于等于0，切换到攻击状态，否则切换到追逐状态
            if (distanceToPlayer <= attackRange && attackCoolDownTime <= 0)
            {
                attackCoolDownTime = attackCoolDown;
                ChangeState(EnemyState.Attacking);
            }
            else if (distanceToPlayer > attackRange && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }

    }
    public void ChangeState(EnemyState newState)
    {
        //退出状态
        if (enemyState == EnemyState.Idle)
        {
            animator.SetBool("isIdle", false);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            animator.SetBool("isChasing", false);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            animator.SetBool("isAttacking", false);
        }
        //更新状态
        enemyState = newState;
        //改变状态
        if (enemyState == EnemyState.Idle)
        {
            animator.SetBool("isIdle", true);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            animator.SetBool("isChasing", true);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            animator.SetBool("isAttacking", true);
        }
    }
    private void OnGrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }
}
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}
