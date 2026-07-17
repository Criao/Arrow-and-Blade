using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家移动控制类，处理玩家的移动、翻转、击退等功能
/// </summary>
public class PlayerMoveMent : MonoBehaviour
{
    private int facingDirection = 1; // 面向方向：1为右，-1为左
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Player_Combat playerCombat;
    [SerializeField] private Player_Bow playerBow; // 弓箭模式引用
    private bool isKnockbacked; // 是否正在被击退
    public bool isShooting; // 是否正在射箭
    private void Update()
    {
        // 检测近战攻击输入
        if (Input.GetButtonDown("Slash") && playerCombat.enabled == true)
        {
            playerCombat.Attack();
        }
    }

    private void FixedUpdate()
    {
        if (isKnockbacked == true) return; // 击退状态下不处理移动

        // 射箭时停止移动
        if (isShooting == true)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("horizontal", 0f);
            animator.SetFloat("vertical", 0f);
            return;
        }

        if (isKnockbacked == false)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // 根据移动方向翻转角色
            if ((horizontal > 0 && transform.localScale.x < 0) || (horizontal < 0 && transform.localScale.x > 0))
            {
                Flip();
            }

            // 只在非弓箭模式下更新动画参数，避免与弓箭动画冲突
            if (playerBow == null || !playerBow.enabled)
            {
                animator.SetFloat("horizontal", Mathf.Abs(horizontal));
                animator.SetFloat("vertical", Mathf.Abs(vertical));
            }

            rb.velocity = new Vector2(horizontal, vertical) * StatsManager.Instance.Speed;
        }
    }

    /// <summary>
    /// 翻转角色朝向
    /// </summary>
    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// 应用击退效果
    /// </summary>
    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockbacked = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    /// <summary>
    /// 击退计时器，结束后恢复正常状态
    /// </summary>
    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        isKnockbacked = false;
    }
}
