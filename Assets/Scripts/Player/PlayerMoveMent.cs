using System.Collections;
using UnityEngine;

public class PlayerMoveMent : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Player_Combat playerCombat;
    [SerializeField] private Player_Bow playerBow;

    private int facingDirection = 1;
    private bool isKnockbacked;
    public bool isShooting;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (playerCombat == null)
        {
            playerCombat = GetComponent<Player_Combat>();
        }

        if (playerBow == null)
        {
            playerBow = GetComponent<Player_Bow>();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Slash") && playerCombat != null && playerCombat.enabled)
        {
            playerCombat.Attack();
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (isKnockbacked)
        {
            return;
        }

        if (isShooting)
        {
            rb.velocity = Vector2.zero;
            SetMoveAnimation(0f, 0f);
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if ((horizontal > 0f && transform.localScale.x < 0f) || (horizontal < 0f && transform.localScale.x > 0f))
        {
            Flip();
        }

        if (!IsBowModeActive())
        {
            SetMoveAnimation(Mathf.Abs(horizontal), Mathf.Abs(vertical));
        }

        float moveSpeed = StatsManager.Instance != null ? StatsManager.Instance.Speed : 0f;
        rb.velocity = new Vector2(horizontal, vertical) * moveSpeed;
    }

    public void SetShooting(bool shooting)
    {
        isShooting = shooting;

        if (!shooting)
        {
            return;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        SetMoveAnimation(0f, 0f);
    }

    public void ClearTemporaryStates()
    {
        isShooting = false;
        SetMoveAnimation(0f, 0f);
    }

    private bool IsBowModeActive()
    {
        return playerBow != null && playerBow.enabled;
    }

    private void SetMoveAnimation(float horizontal, float vertical)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("vertical", vertical);
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        if (enemy == null || rb == null)
        {
            return;
        }

        isKnockbacked = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    private IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        isKnockbacked = false;
    }
}
