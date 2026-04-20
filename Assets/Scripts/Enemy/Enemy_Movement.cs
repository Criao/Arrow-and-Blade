using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// 敌人移动和AI控制类，处理敌人的状态切换、追踪和攻击逻辑
/// </summary>
public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed; // 移动速度
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRange = 2f; // 攻击范围
    [SerializeField] private float attackCoolDown = 2f; // 攻击冷却时间
    [SerializeField] private float playerDetectRange = 5f; // 玩家检测范围
    [SerializeField] private Transform detectionPoint; // 检测点位置
    [SerializeField] private LayerMask playerLayer; // 玩家图层
    [SerializeField] private LayerMask obstacleLayer; // 障碍物图层
    private float attackCoolDownTime; // 攻击冷却计时器
    private EnemyState enemyState; // 当前状态

    private Transform player; // 玩家Transform
    private int facingDirection = 1; // 面向方向

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

    /// <summary>
    /// 追踪玩家
    /// </summary>
    private void Chase()
    {
        // 根据玩家位置调整朝向
        if ((player.position.x > transform.position.x && facingDirection == -1) || (player.position.x < transform.position.x && facingDirection == 1))
        {
            Filp();
        }
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    /// <summary>
    /// 翻转敌人朝向
    /// </summary>
    private void Filp()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// 检测玩家并决定敌人行为
    /// </summary>
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

            // 如果玩家在攻击范围内并且攻击冷却完成，切换到攻击状态
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

    /// <summary>
    /// 切换敌人状态
    /// </summary>
    public void ChangeState(EnemyState newState)
    {
        // 退出当前状态
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

        // 更新状态
        enemyState = newState;

        // 进入新状态
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

/// <summary>
/// 敌人状态枚举
/// </summary>
public enum EnemyState
{
    Idle,      // 待机
    Chasing,   // 追踪
    Attacking, // 攻击
    Knockback  // 击退
}
