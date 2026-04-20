using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人击退类，处理敌人受到击退效果时的行为
/// </summary>
public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
    }

    /// <summary>
    /// 应用击退效果
    /// </summary>
    /// <param name="forceTransform">施加击退力的对象Transform</param>
    /// <param name="knockbackForce">击退力度</param>
    /// <param name="knockbackTime">击退持续时间</param>
    /// <param name="stunTime">眩晕时间</param>
    public void KnockBack(Transform forceTransform,float knockbackForce,float knockbackTime,float stunTime)
    {
        enemy_Movement.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTime(knockbackTime,stunTime));
        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.velocity = direction * knockbackForce;
    }

    /// <summary>
    /// 眩晕计时器，先击退后眩晕，最后恢复待机状态
    /// </summary>
    IEnumerator StunTime(float knockbackTime,float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_Movement.ChangeState(EnemyState.Idle);
    }
}
