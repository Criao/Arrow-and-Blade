using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Settings")]
    public List<QuestData> activeQuests = new List<QuestData>(); // 当前激活的任务列表

    [Header("Equipment Rewards")]
    public List<ItemSo> availableEquipmentRewards = new List<ItemSo>(); // 可用的装备奖励列表

    // 事件：任务状态改变
    public delegate void QuestStatusChanged(QuestData quest);
    public static event QuestStatusChanged OnQuestStatusChanged;

    // 事件：任务完成
    public delegate void QuestCompleted(QuestData quest);
    public static event QuestCompleted OnQuestCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 接受任务
    public void AcceptQuest(string questID, string questName, string description,
                           int requiredKills, RewardType rewardType, int goldReward,
                           string equipmentReward, string enemyTag = "Goblin")
    {
        // 检查是否已经接受过该任务
        if (GetQuest(questID) != null)
        {
            Debug.LogWarning($"任务 {questID} 已经存在！");
            return;
        }

        QuestData newQuest = new QuestData
        {
            questID = questID,
            questName = questName,
            questDescription = description,
            questType = QuestType.Kill,
            status = QuestStatus.InProgress,
            targetEnemyTag = enemyTag,
            requiredKillCount = requiredKills,
            currentKillCount = 0,
            selectedReward = rewardType,
            goldReward = goldReward,
            equipmentRewardID = equipmentReward
        };

        activeQuests.Add(newQuest);
        OnQuestStatusChanged?.Invoke(newQuest);

        Debug.Log($"接受任务: {questName}, 需要击杀 {requiredKills} 个 {enemyTag}");
    }

    // 更新击杀进度
    public void UpdateKillProgress(string enemyTag)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.status == QuestStatus.InProgress &&
                quest.questType == QuestType.Kill &&
                quest.targetEnemyTag == enemyTag)
            {
                quest.currentKillCount++;
                Debug.Log($"任务进度: {quest.currentKillCount}/{quest.requiredKillCount}");

                OnQuestStatusChanged?.Invoke(quest);

                // 检查是否完成
                if (quest.currentKillCount >= quest.requiredKillCount)
                {
                    CompleteQuest(quest.questID);
                }
            }
        }
    }

    // 完成任务
    private void CompleteQuest(string questID)
    {
        QuestData quest = GetQuest(questID);
        if (quest != null && quest.status == QuestStatus.InProgress)
        {
            quest.status = QuestStatus.Completed;
            Debug.Log($"任务完成: {quest.questName}");

            OnQuestCompleted?.Invoke(quest);
            OnQuestStatusChanged?.Invoke(quest);
        }
    }

    // 领取奖励
    public void ClaimReward(string questID)
    {
        QuestData quest = GetQuest(questID);
        if (quest != null && quest.status == QuestStatus.Completed)
        {
            // 根据玩家选择的奖励类型发放奖励
            if (quest.selectedReward == RewardType.Gold)
            {
                GiveGoldReward(quest.goldReward);
            }
            else if (quest.selectedReward == RewardType.Equipment)
            {
                GiveEquipmentReward(quest.equipmentRewardID);
            }

            quest.status = QuestStatus.Rewarded;
            OnQuestStatusChanged?.Invoke(quest);

            // 从激活列表中移除
            activeQuests.Remove(quest);
            Debug.Log($"奖励已领取: {quest.questName}");
        }
    }

    // 发放金币奖励
    private void GiveGoldReward(int amount)
    {
        Debug.Log($"获得金币: {amount}");

        // 直接添加到 InventoryManager
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddGold(amount);
        }
        else
        {
            Debug.LogError("InventoryManager.Instance 为空！无法发放金币奖励");
        }
    }

    // 发放装备奖励
    private void GiveEquipmentReward(string equipmentID)
    {
        Debug.Log($"尝试发放装备: {equipmentID}");

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance 为空！无法发放装备奖励");
            return;
        }

        // 从可用装备列表中查找匹配的装备
        ItemSo equipment = availableEquipmentRewards.Find(item => item.ItemName == equipmentID || item.name == equipmentID);

        if (equipment != null)
        {
            InventoryManager.Instance.AddItem(equipment, 1);
            Debug.Log($"装备 {equipment.ItemName} 已添加到背包");
        }
        else
        {
            Debug.LogError($"无法找到装备: {equipmentID}。请在 QuestManager 的 Available Equipment Rewards 列表中添加该装备");
        }
    }

    // 获取任务
    public QuestData GetQuest(string questID)
    {
        return activeQuests.Find(q => q.questID == questID);
    }

    // 检查任务是否完成
    public bool IsQuestCompleted(string questID)
    {
        QuestData quest = GetQuest(questID);
        return quest != null && quest.status == QuestStatus.Completed;
    }

    // 检查任务是否进行中
    public bool IsQuestInProgress(string questID)
    {
        QuestData quest = GetQuest(questID);
        return quest != null && quest.status == QuestStatus.InProgress;
    }

    // 检查是否有任何活跃的任务（进行中或已完成但未领取奖励）
    public bool HasActiveQuest()
    {
        foreach (var quest in activeQuests)
        {
            if (quest.status == QuestStatus.InProgress || quest.status == QuestStatus.Completed)
            {
                return true;
            }
        }
        return false;
    }
}
