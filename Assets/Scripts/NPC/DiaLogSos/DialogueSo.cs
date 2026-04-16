using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject // ← 改成大写O
{
    public DialogueLine[] lines;

    [Header("Quest Settings")]
    public bool hasQuest; // 是否包含任务
    public string questID; // 任务ID
    public string questDescription; // 任务描述

    [Header("Reward Settings")]
    public int goldReward; // 金币奖励
    public int expReward; // 经验奖励（如果以后需要）
    public string equipmentReward; // 装备奖励ID
}


[System.Serializable]
public class DialogueLine
{
    public ActorSo speaker;
    [SerializeField] [TextArea(3, 5)] public string text;
}
