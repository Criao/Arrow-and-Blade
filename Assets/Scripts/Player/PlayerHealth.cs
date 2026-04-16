using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 玩家健康管理类，负责更新和显示玩家的健康值。
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    /// 显示健康值的文本组件。
    [SerializeField] private TMP_Text healthText;

    /// 健康文本的动画控制器，用于播放更新动画。
    [SerializeField] private Animator healthTextAnimator;
    /// 初始化时设置健康文本。
    private void Start()
    {
        UpdateHealthUI();
    }

    /// 改变玩家的健康值，并更新显示。
    /// <param name="amount">健康值变化量，正数增加，负数减少。</param>
    public void ChangeHealth(int amount)
    {
        ApplyHealthChange(amount);
        healthTextAnimator.Play("TextUpdate");
        UpdateHealthUI();
    }

    /// 更新健康值UI显示
    public void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
        }
    }

    /// 应用健康值变化，并检查是否死亡。
    private void ApplyHealthChange(int amount)
    {
        StatsManager.Instance.CurrentHealth += amount;
        Died();

    }

    /// 检查玩家是否死亡，如果健康值小于等于0，则显示 Game Over。
    private void Died()
    {
        if(StatsManager.Instance.CurrentHealth <= 0)
        {
            StatsManager.Instance.CurrentHealth = 0; // 确保不会是负数
            Debug.Log("玩家死亡！准备显示 Game Over");

            // 显示 Game Over 界面
            if (GameOverManager.Instance != null)
            {
                Debug.Log("找到 GameOverManager，显示 Game Over 界面");
                GameOverManager.Instance.ShowGameOver();
            }
            else
            {
                Debug.LogError("GameOverManager.Instance 为空！请确保场景中有 GameOverManager 对象并添加了脚本");
            }

            gameObject.SetActive(false);
        }
    }
}
