using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    [SerializeField] private NPC npc;
    [SerializeField] private Animator interactionAnimator;
    [SerializeField] private GameObject interactionObj;
    [SerializeField] private DialogueSO dialogueSO;
    [SerializeField] private QuestNPC questNPC;

    private bool playerInRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;

        if (npc != null)
        {
            npc.SetState(NPC.NPCState.Talk);
        }

        if (interactionObj != null)
        {
            interactionObj.SetActive(true);
        }

        if (interactionAnimator != null)
        {
            interactionAnimator.Play("Open");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;

        if (npc != null)
        {
            npc.StartCloseAnimation(interactionAnimator, interactionObj);
            npc.SetState(NPC.NPCState.Patrol);
        }
        else if (interactionObj != null)
        {
            interactionObj.SetActive(false);
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.isDilaogueActive)
        {
            DialogueManager.Instance.EndDialogue();
        }
    }

    private void Update()
    {
        if (!playerInRange || !Input.GetButtonDown("Talk"))
        {
            return;
        }

        if (PauseManager.IsPaused)
        {
            return;
        }

        DialogueManager dialogueManager = DialogueManager.Instance;
        if (dialogueManager == null)
        {
            Debug.LogWarning("DialogueManager.Instance is missing. Cannot start or advance dialogue.");
            return;
        }

        if (dialogueManager.isDilaogueActive)
        {
            dialogueManager.AdvanceDialogue();
            return;
        }

        if (questNPC != null && questNPC.IsQuestCompleted())
        {
            questNPC.OnPlayerInteract();
        }
        else
        {
            dialogueManager.StartDialogue(dialogueSO);
        }
    }
}
