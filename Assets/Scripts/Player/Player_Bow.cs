using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float shootCoolDown = 0.5f;
    [SerializeField] private float ShootTimer;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerMoveMent playerMoveMent;
    private Vector2 aimDirection = Vector2.right;
    private Vector2 lockedAimDirection;

    private void Update()
    {
        ShootTimer -= Time.deltaTime;
        HandleAiming();
        if (Input.GetButtonDown("Shoot") && ShootTimer <= 0)
        {
            lockedAimDirection = aimDirection;
            playerMoveMent.isShooting = true;
            anim.SetBool("isShooting", true);
        }

    }
    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 1);
    }
    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }
    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
        }

        // 始终更新动画参数，确保移动动画正常播放
        anim.SetFloat("aimX", aimDirection.x);
        anim.SetFloat("aimY", aimDirection.y);
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
    }
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
