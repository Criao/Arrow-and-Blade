using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家属性管理器，管理玩家的战斗、移动和生命属性
/// </summary>
public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    [SerializeField] private StatsUI statsUI; // 属性UI引用
    [SerializeField] private TMP_Text healthText; // 生命值文本
    [Header("Combat Stats")]
    [SerializeField] private int damage; // 伤害值
    [SerializeField] private float weaponRange; // 武器范围
    [SerializeField] private float knockbackForce; // 击退力度
    [SerializeField] private float knockbackTime; // 击退时间
    [SerializeField] private float stunTime; // 眩晕时间


    [Header("Movement Stats")]
    [SerializeField] private float speed; // 移动速度

    [Header("Health Stats")]
    [SerializeField] private int currentHealth; // 当前生命值
    [SerializeField] private int maxHealth; // 最大生命值

    /// <summary>
    /// 初始化单例
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUI();
    }

    /// <summary>
    /// 更新最大生命值
    /// </summary>
    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthText();
    }

    /// <summary>
    /// 更新当前生命值
    /// </summary>
    public void UpdateHealth(int amount)
    {
        CurrentHealth += amount;
        UpdateHealthText();

        if (PlayerHealth.ActiveInstance != null)
        {
            PlayerHealth.ActiveInstance.HandleHealthChanged(amount);
        }
    }

    /// <summary>
    /// 更新移动速度
    /// </summary>
    public void UpdateSpeed(int amount)
    {
        speed += amount;
        Debug.Log($"速度更新：{speed - amount} → {speed}");
        UpdateStatsUI();
    }

    /// <summary>
    /// 更新伤害值
    /// </summary>
    public void UpdateDamage(int amount)
    {
        damage += amount;
        Debug.Log($"攻击力更新：{damage - amount} → {damage}");
        UpdateStatsUI();
    }
    #region Combat Stats 访问接口
    /// <summary>
    /// 伤害值属性，限制在1-10之间
    /// </summary>
    public int Damage
    {
        get { return damage; }
        set
        {
            damage = Mathf.Clamp(value, 1, 10);
        }
    }

    /// <summary>
    /// 武器范围
    /// </summary>
    public float WeaponRange => weaponRange;

    /// <summary>
    /// 击退力度
    /// </summary>
    public float KnockbackForce => knockbackForce;

    /// <summary>
    /// 击退时间
    /// </summary>
    public float KnockbackTime => knockbackTime;

    /// <summary>
    /// 眩晕时间
    /// </summary>
    public float StunTime => stunTime;

    #endregion

    #region Movement Stats
    /// <summary>
    /// 移动速度
    /// </summary>
    public float Speed => speed;
    #endregion

    #region Health Stats
    /// <summary>
    /// 当前生命值属性，限制在0到最大生命值之间
    /// </summary>
    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            UpdateHealthText();
        }
    }

    /// <summary>
    /// 最大生命值
    /// </summary>
    public int MaxHealth => maxHealth;
    #endregion

    private void UpdateHealthText()
    {
        EnsureHealthTextReference();

        if (healthText != null)
        {
            healthText.text = "HP:" + currentHealth + "/" + maxHealth;
        }
    }

    private void RefreshUI()
    {
        UpdateHealthText();
        UpdateStatsUI();

        if (PlayerHealth.ActiveInstance != null)
        {
            PlayerHealth.ActiveInstance.UpdateHealthUI();
        }
    }

    private void EnsureHealthTextReference()
    {
        if (healthText != null)
        {
            return;
        }

        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TMP_Text text in texts)
        {
            if (text != null && text.gameObject.name == "HP Text")
            {
                healthText = text;
                return;
            }
        }

        foreach (TMP_Text text in texts)
        {
            if (text != null && text.text.StartsWith("HP:"))
            {
                healthText = text;
                return;
            }
        }
    }

    private void UpdateStatsUI()
    {
        if (statsUI != null)
        {
            statsUI.UpdateAllStats();
        }
    }

}
