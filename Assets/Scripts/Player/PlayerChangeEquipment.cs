using UnityEngine;

public class PlayerChangeEquipment : MonoBehaviour
{
    [SerializeField] private Player_Combat combat;
    [SerializeField] private Player_Bow bow;
    [SerializeField] private PlayerMoveMent movement;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (combat == null)
        {
            combat = GetComponent<Player_Combat>();
        }

        if (bow == null)
        {
            bow = GetComponent<Player_Bow>();
        }

        if (movement == null)
        {
            movement = GetComponent<PlayerMoveMent>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        ApplyMode(bow != null && bow.enabled);
    }

    private void Update()
    {
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            bool bowMode = bow != null && !bow.enabled;
            ApplyMode(bowMode);
        }
    }

    private void ApplyMode(bool bowMode)
    {
        ClearCombatState();

        if (combat != null)
        {
            combat.enabled = !bowMode;
        }

        if (bow != null)
        {
            bow.enabled = bowMode;
            bow.SetBowLayerActive(bowMode);
            bow.ResetBowAnimationState();
        }

        if (movement != null)
        {
            movement.ClearTemporaryStates();
        }

        ResetAnimatorFlags();
    }

    private void ClearCombatState()
    {
        if (combat != null)
        {
            combat.CancelAttack();
        }

        if (bow != null)
        {
            bow.CancelShot();
        }
    }

    private void ResetAnimatorFlags()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("isAttacking", false);
        animator.SetBool("isShooting", false);
        animator.SetFloat("horizontal", 0f);
        animator.SetFloat("vertical", 0f);
    }
}
