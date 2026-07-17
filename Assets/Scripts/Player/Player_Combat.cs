using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float coolDown;
    [SerializeField] private float timer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private StatsUI statsUI;
    [SerializeField] private PlayerMoveMent movement;
    [SerializeField] private float attackStateTimeout = 1.2f;

    private bool isAttackStateActive;
    private float attackStateTimer;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (movement == null)
        {
            movement = GetComponent<PlayerMoveMent>();
        }
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }

        if (isAttackStateActive)
        {
            attackStateTimer += Time.deltaTime;
            if (attackStateTimer >= Mathf.Max(0.1f, attackStateTimeout))
            {
                CancelAttack();
            }
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

        if (movement != null && !movement.StateController.TryStartMeleeAttack())
        {
            return;
        }

        isAttackStateActive = true;
        attackStateTimer = 0f;
        animator.SetBool("isAttacking", true);
        timer = coolDown;
    }

    public void CancelAttack()
    {
        isAttackStateActive = false;
        attackStateTimer = 0f;

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }

        if (movement != null)
        {
            movement.StateController.FinishMeleeAttack();
        }
    }

    private void DealDamage()
    {
        if (!isAttackStateActive)
        {
            return;
        }

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
