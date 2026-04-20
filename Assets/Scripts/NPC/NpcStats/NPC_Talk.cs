using System.Collections;
using UnityEngine;

/// <summary>
/// NPC对话交互，处理玩家与NPC的对话触发
/// </summary>
public class NPC_Talk : MonoBehaviour
{
    [SerializeField] private NPC npc; // NPC控制器
    [SerializeField] private Animator interactionAnimator; // 交互提示动画
    [SerializeField] private GameObject interactionObj; // 交互提示对象
    [SerializeField] private DialogueSO dialogueSO; // 对话数据
    [SerializeField] private QuestNPC questNPC; // 任务NPC组件
    private bool playerInRange = false; // 玩家是否在范围内

    /// <summary>
    /// 玩家进入对话范围
    /// </summary>
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

    /// <summary>
    /// 玩家离开对话范围
    /// </summary>
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

    /// <summary>
    /// 每帧检测玩家输入，处理对话交互
    /// </summary>
    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetButtonDown("Talk"))
        {
            if (DialogueManager.Instance.isDilaogueActive)
            {
                // 对话进行中，推进对话
                DialogueManager.Instance.AdvanceDialogue();
            }
            else
            {
                // 检查是否有任务完成需要领取奖励
                if (questNPC != null && questNPC.IsQuestCompleted())
                {
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