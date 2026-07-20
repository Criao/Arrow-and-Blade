using UnityEngine;

/// <summary>
/// 任务NPC类，处理任务完成后的奖励发放
/// </summary>
public class QuestNPC : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questID; // 该NPC的任务ID
    public DialogueSO questDialogue; // 任务对话
    public DialogueSO rewardDialogue; // 奖励对话

    private bool questCompleted = false; // 任务是否完成

    private void Start()
    {
        // 监听任务完成事件
        QuestManager.OnQuestCompleted += OnQuestCompleted;
    }

    private void OnDestroy()
    {
        QuestManager.OnQuestCompleted -= OnQuestCompleted;
    }

    /// <summary>
    /// 任务完成回调
    /// </summary>
    private void OnQuestCompleted(QuestData quest)
    {
        if (quest.questID == questID)
        {
            questCompleted = true;
        }
    }

    /// <summary>
    /// 玩家与NPC交互时调用（由NPC_Talk调用）
    /// </summary>
    public void OnPlayerInteract()
    {
        if (questCompleted && QuestManager.Instance != null)
        {
            // 任务已完成，发放奖励
            QuestManager.Instance.ClaimReward(questID);
            questCompleted = false;

            // 播放奖励对话（可选）
            if (rewardDialogue != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(rewardDialogue);
            }
        }
    }

    /// <summary>
    /// 检查任务是否完成
    /// </summary>
    public bool IsQuestCompleted()
    {
        return questCompleted;
    }
}
