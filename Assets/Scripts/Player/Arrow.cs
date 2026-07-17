using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeSpawn = 2f;
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite buriedSprite;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private int damage;

    private Coroutine lifeSpanCoroutine;

    public Vector2 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }

    public void Initialize(Vector2 newDirection)
    {
        direction = newDirection.sqrMagnitude > 0f ? newDirection.normalized : Vector2.right;
        ResetArrow();

        if (rb != null)
        {
            rb.velocity = direction * speed;
        }

        RotateArrow();

        if (lifeSpanCoroutine != null)
        {
            StopCoroutine(lifeSpanCoroutine);
        }

        lifeSpanCoroutine = StartCoroutine(LifeSpanTimer());
    }

    private void RotateArrow()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            Enemy_Health enemyHealth = collision.gameObject.GetComponent<Enemy_Health>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
                if (enemyHealth.IsDead)
                {
                    ReturnToPool();
                    return;
                }
            }

            AttachToTarget(collision.transform);
            return;
        }

        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            AttachToTarget(collision.transform);
        }
    }

    private void AttachToTarget(Transform target)
    {
        if (sr != null && buriedSprite != null)
        {
            sr.sprite = buriedSprite;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        transform.SetParent(target);

        if (lifeSpanCoroutine != null)
        {
            StopCoroutine(lifeSpanCoroutine);
        }

        lifeSpanCoroutine = StartCoroutine(DelayedReturn(0.1f));
    }

    private IEnumerator LifeSpanTimer()
    {
        yield return new WaitForSeconds(lifeSpawn);
        ReturnToPool();
    }

    private IEnumerator DelayedReturn(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    private void ResetArrow()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = false;
        }

        if (sr != null && originalSprite != null)
        {
            sr.sprite = originalSprite;
        }

        transform.SetParent(null);
    }

    private void ReturnToPool()
    {
        if (ArrowPool.Instance != null)
        {
            ArrowPool.Instance.ReturnArrow(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (lifeSpanCoroutine != null)
        {
            StopCoroutine(lifeSpanCoroutine);
            lifeSpanCoroutine = null;
        }
    }
}
