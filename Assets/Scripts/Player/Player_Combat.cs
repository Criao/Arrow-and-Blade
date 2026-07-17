using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float coolDown;
    [SerializeField] private float timer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private StatsUI statsUI;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        CancelAttack();
    }

    public void Attack()
    {
        if (timer > 0f || animator == null)
        {
            return;
        }

        animator.SetBool("isAttacking", true);
        timer = coolDown;
    }

    public void CancelAttack()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private void DealDamage()
    {
        StatsManager stats = StatsManager.Instance;
        if (stats == null || attackPoint == null)
        {
            return;
        }

        if (statsUI != null)
        {
            statsUI.UpdateDamage();
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, stats.WeaponRange, enemyLayer);
        if (enemies.Length <= 0)
        {
            return;
        }

        Enemy_Health enemyHealth = enemies[0].GetComponent<Enemy_Health>();
        if (enemyHealth != null)
        {
            enemyHealth.ChangeHealth(-stats.Damage);
        }

        Enemy_Knockback enemyKnockback = enemies[0].GetComponent<Enemy_Knockback>();
        if (enemyKnockback != null)
        {
            enemyKnockback.KnockBack(transform, stats.KnockbackForce, stats.KnockbackTime, stats.StunTime);
        }
    }

    private void FinishAttack()
    {
        CancelAttack();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || StatsManager.Instance == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.WeaponRange);
    }
}
