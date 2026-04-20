using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 玩家生命值管理类，负责更新和显示玩家的生命值
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText; // 生命值文本组件
    [SerializeField] private Animator healthTextAnimator; // 生命值文本动画控制器

    /// <summary>
    /// 初始化时设置生命值文本
    /// </summary>
    private void Start()
    {
        UpdateHealthUI();
    }

    /// <summary>
    /// 改变玩家的生命值，并更新显示
    /// </summary>
    /// <param name="amount">生命值变化量，正数增加，负数减少</param>
    public void ChangeHealth(int amount)
    {
        ApplyHealthChange(amount);
        healthTextAnimator.Play("TextUpdate");
        UpdateHealthUI();
    }

    /// <summary>
    /// 更新生命值UI显示
    /// </summary>
    public void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
        }
    }

    /// <summary>
    /// 应用生命值变化，并检查是否死亡
    /// </summary>
    private void ApplyHealthChange(int amount)
    {
        StatsManager.Instance.CurrentHealth += amount;
        Died();
    }

    /// <summary>
    /// 检查玩家是否死亡，如果生命值小于等于0，则显示Game Over
    /// </summary>
    private void Died()
    {
        if(StatsManager.Instance.CurrentHealth <= 0)
        {
            StatsManager.Instance.CurrentHealth = 0;
            Debug.Log("玩家死亡！准备显示 Game Over");

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
