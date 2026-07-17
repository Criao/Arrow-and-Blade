using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float shootCoolDown = 0.5f;
    [SerializeField] private float ShootTimer;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerMoveMent playerMoveMent;
    [SerializeField] private float shootStateTimeout = 1f;

    private Vector2 aimDirection = Vector2.right;
    private Vector2 lockedAimDirection = Vector2.right;
    private float shootStateTimer;
    private bool isShotStateActive;

    private void Awake()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        if (playerMoveMent == null)
        {
            playerMoveMent = GetComponent<PlayerMoveMent>();
        }
    }

    private void Update()
    {
        if (ShootTimer > 0f)
        {
            ShootTimer -= Time.deltaTime;
        }

        HandleAiming();
        HandleShootStateTimeout();

        if (Input.GetButtonDown("Shoot") && ShootTimer <= 0f)
        {
            StartShot();
        }
    }

    private void OnEnable()
    {
        CacheComponents();
        CancelShot();
        SetBowLayer(true);
    }

    private void OnDisable()
    {
        CancelShot();
        SetBowLayer(false);
    }

    public void CancelShot()
    {
        isShotStateActive = false;
        shootStateTimer = 0f;

        if (playerMoveMent != null)
        {
            playerMoveMent.SetShooting(false);
        }

        if (anim != null)
        {
            anim.SetBool("isShooting", false);
        }
    }

    public void ResetBowAnimationState()
    {
        CancelShot();

        if (anim == null)
        {
            return;
        }

        anim.SetFloat("horizontal", 0f);
        anim.SetFloat("vertical", 0f);
        anim.SetFloat("aimX", aimDirection.x);
        anim.SetFloat("aimY", aimDirection.y);
    }

    public void SetBowLayerActive(bool active)
    {
        SetBowLayer(active);
    }

    private void StartShot()
    {
        if (anim == null)
        {
            Debug.LogWarning("Cannot shoot because the bow animator is missing.");
            return;
        }

        lockedAimDirection = aimDirection;
        isShotStateActive = true;
        shootStateTimer = 0f;

        if (playerMoveMent != null)
        {
            playerMoveMent.SetShooting(true);
        }

        anim.SetBool("isShooting", true);
    }

    private void HandleShootStateTimeout()
    {
        if (!isShotStateActive)
        {
            return;
        }

        shootStateTimer += Time.deltaTime;
        if (shootStateTimer >= Mathf.Max(0.1f, shootStateTimeout))
        {
            CancelShot();
        }
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0f || vertical != 0f)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
        }

        if (anim == null)
        {
            return;
        }

        anim.SetFloat("aimX", aimDirection.x);
        anim.SetFloat("aimY", aimDirection.y);
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
    }

    private void SetBowLayer(bool active)
    {
        if (anim == null || anim.layerCount <= 1)
        {
            return;
        }

        anim.SetLayerWeight(0, active ? 0f : 1f);
        anim.SetLayerWeight(1, active ? 1f : 0f);
    }

    public void Shoot()
    {
        if (ShootTimer <= 0f)
        {
            if (ArrowPool.Instance != null && launchPoint != null)
            {
                ArrowPool.Instance.GetArrow(launchPoint.position, lockedAimDirection);
            }
            else
            {
                Debug.LogWarning("Cannot shoot because ArrowPool or launchPoint is missing.");
            }

            ShootTimer = shootCoolDown;
        }

        CancelShot();
    }
}
