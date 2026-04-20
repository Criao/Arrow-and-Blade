using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能数据ScriptableObject，定义技能的基本属性
/// </summary>
[CreateAssetMenu(fileName = "NewSkill", menuName = "SkillTree/Skill")]
public class SkillSo : ScriptableObject
{
    [SerializeField] private string skillName; // 技能名称
    public string SkillName => skillName;
    [SerializeField] private int maxLevel; // 最大等级
    public int MaxLevel => maxLevel;
    [SerializeField] private Sprite skillIcon; // 技能图标
    public Sprite SkillIcon => skillIcon;
}
