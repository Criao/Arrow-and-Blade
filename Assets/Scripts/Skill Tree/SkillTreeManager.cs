using System.ComponentModel.Design;
using TMPro;
using UnityEngine;

/// <summary>
/// 技能树管理器，管理技能点分配和技能解锁逻辑
/// </summary>
public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] private SkillSlot[] skillSlots; // 所有技能槽位
    [SerializeField] private TMP_Text pointsText; // 技能点显示文本
    [SerializeField] private int availablePoints; // 可用技能点

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
        SkillSlot.OnSkillMaxed += HandleSkillMaxed;
        ExperienceManager.OnLevelUp += UpdateAbilityPoints;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
        SkillSlot.OnSkillMaxed -= HandleSkillMaxed;
        ExperienceManager.OnLevelUp -= UpdateAbilityPoints;
    }

    private void Start()
    {
        // 为每个技能按钮添加点击事件
        foreach (SkillSlot slot in skillSlots)
        {
            slot.SkillButton.onClick.AddListener(() => CheckAvailablePoints(slot));
        }
        UpdateAbilityPoints(0);
    }

    /// <summary>
    /// 检查是否有可用技能点
    /// </summary>
    private void CheckAvailablePoints(SkillSlot slot)
    {
        if(availablePoints  > 0)
        {
            slot.TryUpgradeSkill();
        }
    }

    /// <summary>
    /// 处理技能点消耗
    /// </summary>
    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        if (availablePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }
    }

    /// <summary>
    /// 处理技能满级，检查并解锁后续技能
    /// </summary>
    private void HandleSkillMaxed(SkillSlot skillSlot)
    {
        foreach (SkillSlot slot in skillSlots)
        {
            if (!slot.IsUnlocked && slot.CanUnlockSkill())
            {
                slot.UnlockSkill();
            }
        }
    }

    /// <summary>
    /// 更新可用技能点
    /// </summary>
    private void UpdateAbilityPoints(int amount)
    {
        availablePoints += amount;
        pointsText.text = "Points:  " + availablePoints;
    }
}