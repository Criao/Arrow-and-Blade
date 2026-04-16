using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private List<SkillSlot> prerequisiteSkillSlots;
    [SerializeField] private SkillSo skillSo;
    public SkillSo SkillSo => skillSo;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int currentLevel;
    [SerializeField] private bool isUnlocked;
    [SerializeField] private TMP_Text skilllevelText;
    [SerializeField] private Button skillButton;

    public Button SkillButton => skillButton;
    public bool IsUnlocked => isUnlocked;

    // ✅ 改为 Func，能接收 Manager 返回的 bool
    public static event Action<SkillSlot> OnAbilityPointSpent;
    public static event Action<SkillSlot> OnSkillMaxed;

    private void OnValidate()
    {
        if (skillSo != null && skilllevelText != null)
        {
            UpdateUI();
        }
    }

    // ✅ 改为 public，且名字大写 U 与 Manager 对齐
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

    public void UnlockSkill()
    {
        isUnlocked = true;
        UpdateUI();
    }

    // ✅ 统一入口，删除多余的 UpdateSkill / TryUpGradeSkill 包装
    public void TryUpgradeSkill()
    {
        if (isUnlocked && currentLevel < skillSo.MaxLevel)
        {
            // ✅ 接收返回值，扣点成功才升级
            OnAbilityPointSpent?.Invoke(this);
            currentLevel++;
            
            if (currentLevel >= skillSo.MaxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }
            UpdateUI();
        }
    }

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