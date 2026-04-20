using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// 玩家近战战斗类，处理攻击、伤害判定和冷却
/// </summary>
public class Player_Combat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float coolDown; // 攻击冷却时间
    [SerializeField] private float timer; // 冷却计时器
    [SerializeField] private Transform attackPoint; // 攻击判定点
    [SerializeField] private LayerMask enemyLayer; // 敌人图层
    [SerializeField] private StatsUI statsUI;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 执行攻击
    /// </summary>
    public void Attack()
    {
        if (timer <= 0)
        {
            animator.SetBool("isAttacking", true);
            timer = coolDown;
        }
    }

    /// <summary>
    /// 造成伤害（由动画事件调用）
    /// </summary>
    private void DealDamage()
    {
        statsUI.UpdateDamage();
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.WeaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-StatsManager.Instance.Damage);
            enemies[0].GetComponent<Enemy_Knockback>().KnockBack(transform,StatsManager.Instance.KnockbackForce,StatsManager.Instance.KnockbackTime,StatsManager.Instance.StunTime);
        }
    }

    /// <summary>
    /// 结束攻击动画（由动画事件调用）
    /// </summary>
    private void FinishAttack()
    {
        animator.SetBool("isAttacking", false);
    }

    /// <summary>
    /// 在编辑器中绘制攻击范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || StatsManager.Instance == null) return;
        Gizmos.color  = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,StatsManager.Instance.WeaponRange);
    }
}
