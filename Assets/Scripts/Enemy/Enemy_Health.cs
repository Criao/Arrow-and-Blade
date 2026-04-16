using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private int expReward = 2;
    public delegate void MonsterDefeated(int exp);//委托
    public static event MonsterDefeated OnMonsterDefeateds;//事件


    private void Start()
    {
        currentHealth = maxHealth;
    }


    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if(currentHealth <= 0)
        {
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

