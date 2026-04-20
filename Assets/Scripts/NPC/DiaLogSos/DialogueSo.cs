using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话数据ScriptableObject，存储对话内容和任务信息
/// </summary>
[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] lines; // 对话行数组

    [Header("Quest Settings")]
    public bool hasQuest; // 是否包含任务
    public string questID; // 任务ID
    public string questDescription; // 任务描述

    [Header("Reward Settings")]
    public int goldReward; // 金币奖励
    public int expReward; // 经验奖励（如果以后需要）
    public string equipmentReward; // 装备奖励ID
}

/// <summary>
/// 对话行数据，包含说话者和对话文本
/// </summary>
[System.Serializable]
public class DialogueLine
{
    public ActorSo speaker; // 说话者
    [SerializeField] [TextArea(3, 5)] public string text; // 对话文本
}
