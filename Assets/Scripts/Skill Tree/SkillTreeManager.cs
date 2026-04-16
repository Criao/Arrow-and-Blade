using System.ComponentModel.Design;
using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] private SkillSlot[] skillSlots;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private int availablePoints;


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
        // ✅ 直接注册 TryUpgradeSkill，删除多余的 CheckAvailablePoints
        foreach (SkillSlot slot in skillSlots)
        {
            slot.SkillButton.onClick.AddListener(() => CheckAvailablePoints(slot));
        }
        UpdateAbilityPoints(0);
    }
    private void CheckAvailablePoints(SkillSlot slot)
    {
        if(availablePoints  > 0)
        {
            slot.TryUpgradeSkill();
        }
    }

    // ✅ 类型改为 Func<SkillSlot, bool>，与事件定义匹配
    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        if (availablePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }
    }

    private void HandleSkillMaxed(SkillSlot skillSlot)
    {
        foreach (SkillSlot slot in skillSlots)
        {
            // ✅ CanUnlockSkill 已改为 public，可正常访问
            if (!slot.IsUnlocked && slot.CanUnlockSkill())
            {
                slot.UnlockSkill();
            }
        }
    }

    private void UpdateAbilityPoints(int amount)
    {
        availablePoints += amount;
        pointsText.text = "Points:  " + availablePoints;
    }


}