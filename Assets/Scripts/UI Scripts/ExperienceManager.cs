using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 经验值管理器，处理玩家经验值获取和升级
/// </summary>
public class ExperienceManager : MonoBehaviour
{
    [SerializeField] private int level; // 当前等级
    [SerializeField] private int currentExp; // 当前经验值
    [SerializeField] private int expToLevel = 10; // 升级所需经验值
    [SerializeField] private float expGrowthMultiplier = 1.2f; // 经验值增长倍率
    [SerializeField] private Slider expSlider; // 经验值滑动条
    [SerializeField] private TMP_Text levelText; // 等级文本
    public static event Action<int> OnLevelUp; // 升级事件

    /// <summary>
    /// 初始化，更新UI显示
    /// </summary>
    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// 测试用，按回车键获得经验值
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GainExperience(2);
        }
    }

    /// <summary>
    /// 启用时监听敌人死亡事件
    /// </summary>
    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeateds += GainExperience;
    }

    /// <summary>
    /// 禁用时取消监听
    /// </summary>
    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeateds -= GainExperience;
    }

    /// <summary>
    /// 获得经验值
    /// </summary>
    private void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    /// <summary>
    /// 升级
    /// </summary>
    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);
        OnLevelUp?.Invoke(1);
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;
        levelText.text = "Level:" + level;
    }
}
