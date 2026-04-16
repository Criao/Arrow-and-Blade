using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "SkillTree/Skill")]
public class SkillSo : ScriptableObject
{
    [SerializeField] private string skillName;
    public string SkillName => skillName; 
    [SerializeField] private int maxLevel;
    public int MaxLevel => maxLevel;
    [SerializeField] private Sprite skillIcon; 
    public Sprite SkillIcon => skillIcon;
}
