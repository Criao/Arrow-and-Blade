using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questID; // 该NPC的任务ID
    public DialogueSO questDialogue; // 任务对话
    public DialogueSO rewardDialogue; // 奖励对话

    private bool questCompleted = false;

    private void Start()
    {
        // 监听任务完成事件
        QuestManager.OnQuestCompleted += OnQuestCompleted;
    }

    private void OnDestroy()
    {
        QuestManager.OnQuestCompleted -= OnQuestCompleted;
    }

    // 任务完成回调
    private void OnQuestCompleted(QuestData quest)
    {
        if (quest.questID == questID)
        {
            questCompleted = true;
            Debug.Log($"NPC {gameObject.name}: 任务已完成，等待玩家领取奖励");
        }
    }

    // 玩家与NPC交互时调用（由NPC_Talk调用）
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

    // 检查任务是否完成
    public bool IsQuestCompleted()
    {
        return questCompleted;
    }
}
