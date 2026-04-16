using UnityEngine;

[System.Serializable]
public class QuestData
{
    public string questID;
    public string questName;
    public string questDescription;
    public QuestType questType;
    public QuestStatus status;

    [Header("Kill Quest Settings")]
    public string targetEnemyTag = "Goblin"; // 目标敌人标签
    public int requiredKillCount; // 需要击杀的数量
    public int currentKillCount; // 当前击杀数量

    [Header("Reward Settings")]
    public RewardType selectedReward; // 玩家选择的奖励类型
    public int goldReward;
    public string equipmentRewardID; // 装备ID
}

public enum QuestType
{
    Kill, // 击杀任务
    Collect, // 收集任务
    Talk // 对话任务
}

public enum QuestStatus
{
    NotStarted, // 未开始
    InProgress, // 进行中
    Completed, // 已完成
    Rewarded // 已领取奖励
}

public enum RewardType
{
    Gold, // 金币奖励
    Equipment // 装备奖励
}
