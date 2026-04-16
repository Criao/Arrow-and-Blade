using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float weaponRange;
    [SerializeField] private float knockbackForce;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float stunTime;
    private void Attack()
    {
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
