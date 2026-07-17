using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能效果管理器，根据技能名称应用对应的效果
/// </summary>
public class SkillManager : MonoBehaviour
{
    [SerializeField] private Player_Combat combat; // 玩家战斗组件

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }

    /// <summary>
    /// 处理技能点消耗，应用技能效果
    /// </summary>
    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        if (slot == null || slot.SkillSo == null)
        {
            return;
        }

        string skillName = slot.SkillSo.SkillName;
        switch (skillName)
        {
            case "Max Health Boost":
                if (StatsManager.Instance != null)
                {
                    StatsManager.Instance.UpdateMaxHealth(1);
                }
                break;
            case "Sword Slash":
                if (combat != null)
                {
                    combat.enabled = true;
                }
                break;
            default:
                Debug.LogWarning("UnKown Skill: " + skillName);
                break;
        }
    }
}
