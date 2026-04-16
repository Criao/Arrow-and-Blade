using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;
using System;

public class ExperienceManager : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private int currentExp;
    [SerializeField] private int expToLevel = 10;
    [SerializeField] private float expGrowthMultiplier = 1.2f;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text levelText;
    public static event Action<int> OnLevelUp;
    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GainExperience(2);
        }
    }
    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeateds += GainExperience;
    }
    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeateds -= GainExperience;
    }
    private void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateUI();

    }
    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);
        OnLevelUp?.Invoke(1);
    }
    private void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;
        levelText.text = "Level:" + level;
    }

}
