using System.Collections;
using UnityEngine;

public class PlayerMoveMent : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Player_Combat playerCombat;
    [SerializeField] private Player_Bow playerBow;

    private readonly PlayerStateController stateController = new PlayerStateController();
    private int facingDirection = 1;
    private Coroutine knockbackRoutine;

    public bool isShooting;
    public PlayerState CurrentState => stateController.CurrentState;
    public PlayerStateController StateController => stateController;

    private void Awake()
    {
        CacheComponents();
    }

    private void CacheComponents()
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
        if (PauseManager.IsPaused)
        {
            return;
        }

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

        if (stateController.CurrentState == PlayerState.Dead)
        {
            rb.velocity = Vector2.zero;
            SetMoveAnimation(0f, 0f);
            return;
        }

        Vector2 input = GetMovementInput();

        if (stateController.CurrentState == PlayerState.Knockback)
        {
            SetMoveAnimation(0f, 0f);
            return;
        }

        if (stateController.IsMovementLocked)
        {
            rb.velocity = Vector2.zero;
            SetMoveAnimation(0f, 0f);
            return;
        }

        if (input.x > 0f && transform.localScale.x < 0f || input.x < 0f && transform.localScale.x > 0f)
        {
            Flip();
        }

        if (!stateController.IsBowMode)
        {
            SetMoveAnimation(Mathf.Abs(input.x), Mathf.Abs(input.y));
        }

        float moveSpeed = StatsManager.Instance != null ? StatsManager.Instance.Speed : 0f;
        rb.velocity = input * moveSpeed;
        stateController.UpdateLocomotion(input);
    }

    public void SetEquipmentMode(bool bowMode)
    {
        stateController.SetEquipmentMode(bowMode ? PlayerEquipmentMode.Bow : PlayerEquipmentMode.Melee);
    }

    public void SetShooting(bool shooting)
    {
        isShooting = shooting;

        if (shooting)
        {
            if (stateController.CurrentState != PlayerState.BowShooting && !stateController.TryStartBowShot())
            {
                isShooting = false;
                return;
            }

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }

            SetMoveAnimation(0f, 0f);
        }
        else
        {
            stateController.FinishBowShot();
        }
    }

    public void ClearTemporaryStates()
    {
        isShooting = false;
        stateController.ClearTemporaryStates(GetMovementInput());
        SetMoveAnimation(0f, 0f);
    }

    public void SetDead()
    {
        isShooting = false;
        playerBow?.CancelShot();
        playerCombat?.CancelAttack();
        StopKnockbackRoutine();
        stateController.SetDead();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        SetMoveAnimation(0f, 0f);
    }

    public void Revive()
    {
        isShooting = false;
        playerBow?.CancelShot();
        playerCombat?.CancelAttack();
        StopKnockbackRoutine();
        stateController.Revive();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isShooting", false);
        }

        SetMoveAnimation(0f, 0f);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        if (enemy == null || rb == null || stateController.CurrentState == PlayerState.Dead)
        {
            return;
        }

        isShooting = false;
        playerBow?.CancelShot();
        playerCombat?.CancelAttack();
        stateController.StartKnockback();

        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        HitFeedbackAudio.PlayKnockbackImpact(transform.position);
        StopKnockbackRoutine();
        knockbackRoutine = StartCoroutine(KnockbackCounter(stunTime));
    }

    private Vector2 GetMovementInput()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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

    private IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        stateController.FinishKnockback(GetMovementInput());
        knockbackRoutine = null;
    }

    private void StopKnockbackRoutine()
    {
        if (knockbackRoutine == null)
        {
            return;
        }

        StopCoroutine(knockbackRoutine);
        knockbackRoutine = null;
    }
}

public enum PlayerState
{
    Idle,
    Moving,
    MeleeAttack,
    BowAiming,
    BowShooting,
    Knockback,
    Dead
}

public enum PlayerEquipmentMode
{
    Melee,
    Bow
}

public class PlayerStateController
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    public PlayerEquipmentMode EquipmentMode { get; private set; } = PlayerEquipmentMode.Melee;

    public bool IsBowMode => EquipmentMode == PlayerEquipmentMode.Bow;
    public bool IsMovementLocked =>
        CurrentState == PlayerState.MeleeAttack ||
        CurrentState == PlayerState.BowShooting ||
        CurrentState == PlayerState.Knockback ||
        CurrentState == PlayerState.Dead;

    public bool CanStartMeleeAttack =>
        !IsBowMode &&
        (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Moving);

    public bool CanStartBowShot =>
        IsBowMode &&
        (CurrentState == PlayerState.Idle ||
         CurrentState == PlayerState.Moving ||
         CurrentState == PlayerState.BowAiming);

    public void SetEquipmentMode(PlayerEquipmentMode mode)
    {
        if (CurrentState == PlayerState.Dead)
        {
            return;
        }

        EquipmentMode = mode;

        if (CurrentState == PlayerState.Knockback)
        {
            return;
        }

        TransitionTo(IsBowMode ? PlayerState.BowAiming : PlayerState.Idle);
    }

    public void UpdateLocomotion(Vector2 movementInput)
    {
        if (IsMovementLocked)
        {
            return;
        }

        bool isMoving = movementInput.sqrMagnitude > 0.0001f;
        if (IsBowMode)
        {
            TransitionTo(isMoving ? PlayerState.Moving : PlayerState.BowAiming);
        }
        else
        {
            TransitionTo(isMoving ? PlayerState.Moving : PlayerState.Idle);
        }
    }

    public bool TryStartMeleeAttack()
    {
        if (!CanStartMeleeAttack)
        {
            return false;
        }

        TransitionTo(PlayerState.MeleeAttack);
        return true;
    }

    public bool TryStartBowShot()
    {
        if (!CanStartBowShot)
        {
            return false;
        }

        TransitionTo(PlayerState.BowShooting);
        return true;
    }

    public void FinishMeleeAttack()
    {
        if (CurrentState == PlayerState.MeleeAttack)
        {
            TransitionTo(IsBowMode ? PlayerState.BowAiming : PlayerState.Idle);
        }
    }

    public void FinishBowShot()
    {
        if (CurrentState == PlayerState.BowShooting)
        {
            TransitionTo(PlayerState.BowAiming);
        }
    }

    public void StartKnockback()
    {
        if (CurrentState != PlayerState.Dead)
        {
            TransitionTo(PlayerState.Knockback);
        }
    }

    public void FinishKnockback(Vector2 movementInput)
    {
        if (CurrentState == PlayerState.Knockback)
        {
            TransitionTo(IsBowMode ? PlayerState.BowAiming : PlayerState.Idle);
            UpdateLocomotion(movementInput);
        }
    }

    public void ClearTemporaryStates(Vector2 movementInput)
    {
        if (CurrentState == PlayerState.Dead || CurrentState == PlayerState.Knockback)
        {
            return;
        }

        TransitionTo(IsBowMode ? PlayerState.BowAiming : PlayerState.Idle);
        UpdateLocomotion(movementInput);
    }

    public void SetDead()
    {
        TransitionTo(PlayerState.Dead);
    }

    public void Revive()
    {
        TransitionTo(IsBowMode ? PlayerState.BowAiming : PlayerState.Idle);
    }

    private void TransitionTo(PlayerState newState)
    {
        CurrentState = newState;
    }
}
