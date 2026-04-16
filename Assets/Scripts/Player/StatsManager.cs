using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    [SerializeField] private StatsUI statsUI; 
    [SerializeField] private TMP_Text healthText;
    [Header("Combat Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float weaponRange;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackTime;
    [SerializeField] private float stunTime;
    

    [Header("Movement Stats")]
    [SerializeField] private float speed;

    [Header("Health Stats")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        healthText.text = "HP:" + currentHealth + "/" + maxHealth;
    }
     public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth >= maxHealth)
            currentHealth = maxHealth;

            
        healthText.text = "HP:" + currentHealth + "/" + maxHealth;
    } 
     public void UpdateSpeed(int amount)
    {
        speed += amount;
        Debug.Log($"速度更新：{speed - amount} → {speed}");
        statsUI.UpdateAllStats();
    }
    public void UpdateDamage(int amount)
    {
        damage += amount;
        Debug.Log($"攻击力更新：{damage - amount} → {damage}");
        statsUI.UpdateAllStats();
    }
    #region Combat Stats 访问接口
    public int Damage
    {
        /// 获取当前伤害值。
        get { return damage; }
        /// 设置当前伤害值，使用 Mathf.Clamp 确保值在1到10 之间。
        set
        {
            damage = Mathf.Clamp(value, 1, 10);
        }
    }
    public float WeaponRange => weaponRange;
    public float KnockbackForce => knockbackForce;
    public float KnockbackTime => knockbackTime;
    public float StunTime => stunTime;

    #endregion

    #region Movement Stats
    public float Speed => speed;
    #endregion

    #region Health Stats
    /// 获取或设置当前健康值。设置时会自动限制在0到最大健康值之间。
    public int CurrentHealth
    {
        /// 获取当前健康值。
        get { return currentHealth; }
        /// 设置当前健康值，使用 Mathf.Clamp 确保值在0到 maxHealth 之间。
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
        }
    }
    public int MaxHealth => maxHealth;
    #endregion


}
