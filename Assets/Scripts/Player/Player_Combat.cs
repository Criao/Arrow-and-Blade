using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float coolDown;
    [SerializeField] private float timer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private StatsUI statsUI;
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
    public void Attack()
    {
        if (timer <= 0)
        {
            animator.SetBool("isAttacking", true);
            timer = coolDown;
        }

    }
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
    private void FinishAttack()
    {
        animator.SetBool("isAttacking", false);
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || StatsManager.Instance == null) return;
        Gizmos.color  = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,StatsManager.Instance.WeaponRange);
    }
}
