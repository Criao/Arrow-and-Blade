using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 玩家弓箭战斗类，处理瞄准和射箭功能
/// </summary>
public class Player_Bow : MonoBehaviour
{
    [SerializeField] private Transform launchPoint; // 箭矢发射点
    [SerializeField] private float shootCoolDown = 0.5f; // 射击冷却时间
    [SerializeField] private float ShootTimer; // 射击计时器
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerMoveMent playerMoveMent;
    private Vector2 aimDirection = Vector2.right; // 当前瞄准方向
    private Vector2 lockedAimDirection; // 锁定的瞄准方向（射击时）

    private void Update()
    {
        ShootTimer -= Time.deltaTime;
        HandleAiming();

        // 按下射击键时，锁定瞄准方向并开始射击动画
        if (Input.GetButtonDown("Shoot") && ShootTimer <= 0)
        {
            lockedAimDirection = aimDirection;
            playerMoveMent.isShooting = true;
            anim.SetBool("isShooting", true);
        }
    }

    /// <summary>
    /// 启用弓箭模式时，切换动画层
    /// </summary>
    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0); // 禁用近战动画层
        anim.SetLayerWeight(1, 1); // 启用弓箭动画层
    }

    /// <summary>
    /// 禁用弓箭模式时，恢复近战动画层
    /// </summary>
    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }

    /// <summary>
    /// 处理瞄准方向和动画参数
    /// </summary>
    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 更新瞄准方向
        if (horizontal != 0 || vertical != 0)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
        }

        // 更新动画参数
        anim.SetFloat("aimX", aimDirection.x);
        anim.SetFloat("aimY", aimDirection.y);
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
    }

    /// <summary>
    /// 射出箭矢（由动画事件调用）
    /// </summary>
    public void Shoot()
    {
        if (ShootTimer <= 0)
        {
            ArrowPool.Instance.GetArrow(launchPoint.position, lockedAimDirection);
            ShootTimer = shootCoolDown;
        }
        anim.SetBool("isShooting", false);
        playerMoveMent.isShooting = false;
    }
}
