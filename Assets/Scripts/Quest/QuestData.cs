using UnityEngine;

/// <summary>
/// 任务数据类，存储任务的所有信息
/// </summary>
[System.Serializable]
public class QuestData
{
    public string questID; // 任务唯一ID
    public string questName; // 任务名称
    public string questDescription; // 任务描述
    public QuestType questType; // 任务类型
    public QuestStatus status; // 任务状态

    [Header("Kill Quest Settings")]
    public string targetEnemyTag = "Goblin"; // 目标敌人标签
    public int requiredKillCount; // 需要击杀的数量
    public int currentKillCount; // 当前击杀数量

    [Header("Reward Settings")]
    public RewardType selectedReward; // 玩家选择的奖励类型
    public int goldReward; // 金币奖励
    public string equipmentRewardID; // 装备ID
}

/// <summary>
/// 任务类型枚举
/// </summary>
public enum QuestType
{
    Kill,    // 击杀任务
    Collect, // 收集任务
    Talk     // 对话任务
}

/// <summary>
/// 任务状态枚举
/// </summary>
public enum QuestStatus
{
    NotStarted, // 未开始
    InProgress, // 进行中
    Completed,  // 已完成
    Rewarded    // 已领取奖励
}

/// <summary>
/// 奖励类型枚举
/// </summary>
public enum RewardType
{
    Gold,      // 金币奖励
    Equipment  // 装备奖励
}
