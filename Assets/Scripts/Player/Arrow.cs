using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭矢类，处理箭矢的飞行、碰撞、伤害和对象池回收
/// </summary>
public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 15f; // 箭矢飞行速度
    [SerializeField] private float lifeSpawn = 2f; // 箭矢生命周期
    [SerializeField] private Vector2 direction = Vector2.right; // 飞行方向
    [SerializeField] private LayerMask enemyLayer; // 敌人图层
    [SerializeField] private LayerMask obstacleLayer; // 障碍物图层
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite buriedSprite; // 插入目标后的精灵
    [SerializeField] private Sprite originalSprite; // 原始精灵
    [SerializeField] private int damage; // 伤害值
    [SerializeField] private float knockbackForce; // 击退力度
    [SerializeField] private float knockbackTime; // 击退时间
    [SerializeField] private float stunTime; // 眩晕时间

    private Coroutine lifeSpanCoroutine;

    public Vector2 Direction
    {
        get{return direction;}
        set{direction = value;}
    }

    /// <summary>
    /// 初始化箭矢，设置方向和速度
    /// </summary>
    public void Initialize(Vector2 newDirection)
    {
        direction = newDirection;
        ResetArrow();
        rb.velocity = direction * speed;
        RotateArrow();

        if (lifeSpanCoroutine != null)
        {
            StopCoroutine(lifeSpanCoroutine);
        }
        lifeSpanCoroutine = StartCoroutine(LifeSpanTimer());
    }

    /// <summary>
    /// 根据飞行方向旋转箭矢
    /// </summary>
    private void RotateArrow()
    {
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
    }

    /// <summary>
    /// 碰撞检测，处理击中敌人或障碍物
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((enemyLayer & (1 << collision.gameObject.layer)) > 0)
        {
            collision.gameObject.GetComponent<Enemy_Health>().ChangeHealth(-damage);
            collision.gameObject.GetComponent<Enemy_Knockback>().KnockBack(transform,knockbackForce,knockbackTime,stunTime);
            AttachToTarget(collision.gameObject.transform);
        }
        else if((obstacleLayer & (1 << collision.gameObject.layer)) > 0)
        {
            AttachToTarget(collision.gameObject.transform);
        }
    }

    /// <summary>
    /// 将箭矢附着到目标上
    /// </summary>
    private void AttachToTarget(Transform target)
    {
        sr.sprite = buriedSprite;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.SetParent(target);

        if (lifeSpanCoroutine != null)
        {
            StopCoroutine(lifeSpanCoroutine);
        }
        lifeSpanCoroutine = StartCoroutine(DelayedReturn(0.1f));
    }

    /// <summary>
    /// 生命周期计时器
    /// </summary>
    private IEnumerator LifeSpanTimer()
    {
        yield return new WaitForSeconds(lifeSpawn);
        ReturnToPool();
    }

    /// <summary>
    /// 延迟返回对象池
    /// </summary>
    private IEnumerator DelayedReturn(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    /// <summary>
    /// 重置箭矢状态
    /// </summary>
    private void ResetArrow()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;

        if (originalSprite != null)
        {
            sr.sprite = originalSprite;
        }

        transform.SetParent(null);
    }

    /// <summary>
    /// 返回对象池
    /// </summary>
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
