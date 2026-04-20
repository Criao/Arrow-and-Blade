using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// 技能槽位类，管理单个技能的状态、升级和UI显示
/// </summary>
public class SkillSlot : MonoBehaviour
{
    [SerializeField] private List<SkillSlot> prerequisiteSkillSlots; // 前置技能槽位列表
    [SerializeField] private SkillSo skillSo; // 技能数据
    public SkillSo SkillSo => skillSo;
    [SerializeField] private Image skillIcon; // 技能图标
    [SerializeField] private int currentLevel; // 当前等级
    [SerializeField] private bool isUnlocked; // 是否已解锁
    [SerializeField] private TMP_Text skilllevelText; // 等级文本
    [SerializeField] private Button skillButton; // 技能按钮

    public Button SkillButton => skillButton;
    public bool IsUnlocked => isUnlocked;

    public static event Action<SkillSlot> OnAbilityPointSpent; // 技能点消耗事件
    public static event Action<SkillSlot> OnSkillMaxed; // 技能满级事件

    private void OnValidate()
    {
        if (skillSo != null && skilllevelText != null)
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// 检查是否可以解锁该技能（前置技能是否满足）
    /// </summary>
    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisiteSkillSlots)
        {
            if (!slot.isUnlocked || slot.currentLevel < slot.skillSo.MaxLevel)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 解锁技能
    /// </summary>
    public void UnlockSkill()
    {
        isUnlocked = true;
        UpdateUI();
    }

    /// <summary>
    /// 尝试升级技能
    /// </summary>
    public void TryUpgradeSkill()
    {
        if (isUnlocked && currentLevel < skillSo.MaxLevel)
        {
            OnAbilityPointSpent?.Invoke(this);
            currentLevel++;

            if (currentLevel >= skillSo.MaxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }
            UpdateUI();
        }
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        if (skillSo == null)
        {
            Debug.LogError($"{name}: skillSo is NULL");
            return;
        }

        if (skillIcon == null)
        {
            Debug.LogError($"{name}: skillIcon is NULL");
            return;
        }

        if (skilllevelText == null)
        {
            Debug.LogError($"{name}: skilllevelText is NULL");
            return;
        }

        if (skillButton == null)
        {
            Debug.LogError($"{name}: skillButton is NULL");
            return;
        }

        skillIcon.sprite = skillSo.SkillIcon;

        if (isUnlocked)
        {
            skillButton.interactable = true;
            skilllevelText.text = currentLevel + "/" + skillSo.MaxLevel;
            skilllevelText.color = Color.white;
        }
        else
        {
            skillButton.interactable = false;
            skilllevelText.text = "Locked";
            skilllevelText.color = Color.grey;
        }
    }
}