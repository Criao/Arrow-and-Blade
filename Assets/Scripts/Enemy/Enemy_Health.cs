using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生命值管理类，处理敌人的生命值变化和死亡逻辑
/// </summary>
public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private int currentHealth; // 当前生命值
    [SerializeField] private int maxHealth; // 最大生命值
    [SerializeField] private int expReward = 2; // 击败后给予的经验值奖励

    public delegate void MonsterDefeated(int exp); // 怪物被击败的委托
    public static event MonsterDefeated OnMonsterDefeateds; // 怪物被击败的事件

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 改变生命值
    /// </summary>
    /// <param name="amount">变化量，正数为治疗，负数为伤害</param>
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if(currentHealth <= 0)
        {
            // 触发怪物被击败事件，发放经验值
            OnMonsterDefeateds(expReward);

            // 通知任务系统敌人被击杀
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateKillProgress(gameObject.tag);
            }

            Destroy(this.gameObject);
        }
    }
}

