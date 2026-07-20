using UnityEngine;
using TMPro;

/// <summary>
/// 任务UI类，显示任务进度和状态
/// </summary>
public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questPanel; // 任务面板
    public TMP_Text questTitleText; // 任务标题
    public TMP_Text questProgressText; // 任务进度

    private void Start()
    {
        // 监听任务状态变化
        QuestManager.OnQuestStatusChanged += UpdateQuestUI;
        QuestManager.OnQuestCompleted += OnQuestCompleted;

        // 初始隐藏任务面板
        if (questPanel != null)
            questPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // 取消监听
        QuestManager.OnQuestStatusChanged -= UpdateQuestUI;
        QuestManager.OnQuestCompleted -= OnQuestCompleted;
    }

    /// <summary>
    /// 更新任务UI显示
    /// </summary>
    private void UpdateQuestUI(QuestData quest)
    {
        if (quest.status == QuestStatus.InProgress)
        {
            // 显示任务面板
            if (questPanel != null)
                questPanel.SetActive(true);

            // 显示任务标题
            if (questTitleText != null)
            {
                questTitleText.gameObject.SetActive(true);
                questTitleText.text = quest.questName;
            }

            // 更新进度显示
            if (questProgressText != null)
            {
                // 检查是否达到目标
                if (quest.currentKillCount >= quest.requiredKillCount)
                {
                    questProgressText.text = $"{quest.requiredKillCount}/{quest.requiredKillCount}";
                }
                else
                {
                    questProgressText.text = $"{quest.currentKillCount}/{quest.requiredKillCount}";
                }
            }
        }
        else if (quest.status == QuestStatus.Completed)
        {
            // 任务完成，隐藏任务标题，只显示 "Mission Completed"
            if (questTitleText != null)
                questTitleText.gameObject.SetActive(false);

            if (questProgressText != null)
                questProgressText.text = "Mission Completed";
        }
        else if (quest.status == QuestStatus.Rewarded)
        {
            // 奖励已领取，隐藏任务面板
            if (questPanel != null)
                questPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 任务完成时的回调
    /// </summary>
    private void OnQuestCompleted(QuestData quest)
    {
    }
}
