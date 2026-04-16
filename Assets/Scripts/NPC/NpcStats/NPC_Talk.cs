using System.Collections;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    [SerializeField] private NPC npc;
    [SerializeField] private Animator interactionAnimator;
    [SerializeField] private GameObject interactionObj;
    [SerializeField] private DialogueSO dialogueSO;
    [SerializeField] private QuestNPC questNPC; // 任务NPC组件
    private bool playerInRange = false; // ← 玩家是否在范围内

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = true;
            npc.SetState(NPC.NPCState.Talk);
            interactionObj.SetActive(true);
            interactionAnimator.Play("Open");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = false;
            npc.StartCloseAnimation(interactionAnimator, interactionObj);
            npc.SetState(NPC.NPCState.Patrol);

            // 玩家离开时如果对话还开着，关闭对话
            if (DialogueManager.Instance != null && DialogueManager.Instance.isDilaogueActive)
            {
                DialogueManager.Instance.EndDialogue();
            }
        }
    }

    private void Update()
    {
        if (!playerInRange) return; // ← 不在范围内不响应

        if (Input.GetButtonDown("Talk"))
        {
            if (DialogueManager.Instance.isDilaogueActive)
            {
                DialogueManager.Instance.AdvanceDialogue();
            }
            else
            {
                // 检查是否有任务完成需要领取奖励
                if (questNPC != null && questNPC.IsQuestCompleted())
                {
                    // 任务完成，领取奖励
                    questNPC.OnPlayerInteract();
                }
                else
                {
                    // 正常对话
                    DialogueManager.Instance.StartDialogue(dialogueSO);
                }
            }
        }
    }
}