using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人战斗类，处理敌人的攻击判定和伤害
/// </summary>
public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private int damage = 1; // 攻击伤害
    [SerializeField] private Transform attackPoint; // 攻击判定点
    [SerializeField] private float weaponRange; // 武器范围
    [SerializeField] private float knockbackForce; // 击退力度
    [SerializeField] private LayerMask playerLayer; // 玩家图层
    [SerializeField] private float stunTime; // 眩晕时间

    /// <summary>
    /// 执行攻击（由动画事件调用）
    /// </summary>
    private void Attack()
    {
       if (GameManager.IsSceneTransitionBlocked)
       {
           return;
       }

       if (attackPoint == null)
       {
           Debug.LogWarning($"{nameof(Enemy_Combat)} cannot attack because attackPoint is missing.");
           return;
       }

       Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position,weaponRange,playerLayer);
       if(hits.Length > 0)
        {
            PlayerHealth playerHealth = hits[0].GetComponent<PlayerHealth>();
            PlayerMoveMent playerMovement = hits[0].GetComponent<PlayerMoveMent>();

            // 检查组件是否存在且所在对象是否激活
            if (playerHealth != null && playerHealth.gameObject.activeInHierarchy)
                playerHealth.ChangeHealth(-damage);

            if (playerMovement != null && playerMovement.gameObject.activeInHierarchy)
                playerMovement.Knockback(transform, knockbackForce, stunTime);
        }
    }
}
