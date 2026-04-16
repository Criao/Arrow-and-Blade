using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMoveMent : MonoBehaviour
{
    private int facingDirection = 1;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Player_Combat playerCombat;
    [SerializeField] private Player_Bow playerBow; // 添加弓箭手引用
    private bool isKnockbacked;
    public bool isShooting;
    private void Update()
    {
        if (Input.GetButtonDown("Slash") && playerCombat.enabled == true)
        {
            playerCombat.Attack();
        }
    }

    private void FixedUpdate()
    {
        if (isKnockbacked == true) return;
        if (isShooting == true)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("horizontal", 0f);
            animator.SetFloat("vertical", 0f);
            return; // 射箭时直接返回，不处理翻转
        }

        if (isKnockbacked == false)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if ((horizontal > 0 && transform.localScale.x < 0) || (horizontal < 0 && transform.localScale.x > 0))
            {
                Flip();
            }

            // 只在非弓箭手模式下更新动画参数，避免冲突
            if (playerBow == null || !playerBow.enabled)
            {
                animator.SetFloat("horizontal", Mathf.Abs(horizontal));
                animator.SetFloat("vertical", Mathf.Abs(vertical));
            }

            rb.velocity = new Vector2(horizontal, vertical) * StatsManager.Instance.Speed;
        }
    }
    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockbacked = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));

    }
    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        isKnockbacked = false;
    }
}
