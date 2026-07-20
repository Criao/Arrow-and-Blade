using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private Coroutine sceneLoadedRefreshCoroutine;

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
        QueueSceneLoadedRefresh();
    }

    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        RefreshHealthUI();
    }

    public void UpdateHealth(int amount)
    {
        CurrentHealth += amount;

        if (PlayerHealth.ActiveInstance != null)
        {
            PlayerHealth.ActiveInstance.HandleHealthChanged(amount);
        }
    }

    public void UpdateSpeed(int amount)
    {
        speed += amount;
        UpdateStatsUI();
    }

    public void UpdateDamage(int amount)
    {
        damage += amount;
        UpdateStatsUI();
    }

    public int Damage
    {
        get { return damage; }
        set { damage = Mathf.Clamp(value, 1, 10); }
    }

    public float WeaponRange => weaponRange;

    public float KnockbackForce => knockbackForce;

    public float KnockbackTime => knockbackTime;

    public float StunTime => stunTime;

    public float Speed => speed;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            RefreshHealthUI();
        }
    }

    public int MaxHealth => maxHealth;

    public void RefreshHealthUI()
    {
        UpdateHealthText();
    }

    private void RefreshUI()
    {
        RefreshHealthUI();
        UpdateStatsUI();
    }

    private void QueueSceneLoadedRefresh()
    {
        if (!gameObject.activeInHierarchy)
        {
            RefreshUI();
            return;
        }

        if (sceneLoadedRefreshCoroutine != null)
        {
            StopCoroutine(sceneLoadedRefreshCoroutine);
        }

        sceneLoadedRefreshCoroutine = StartCoroutine(RefreshUIAfterSceneLoad());
    }

    private IEnumerator RefreshUIAfterSceneLoad()
    {
        yield return null;
        RefreshUI();
        sceneLoadedRefreshCoroutine = null;
    }

    private void UpdateHealthText()
    {
        string healthTextValue = "HP:" + currentHealth + "/" + maxHealth;
        TMP_Text preferredText = FindAndUpdateHealthTexts(healthTextValue);

        if (preferredText != null)
        {
            healthText = preferredText;
            return;
        }

        if (healthText != null)
        {
            healthText.text = healthTextValue;
        }
    }

    private TMP_Text FindAndUpdateHealthTexts(string healthTextValue)
    {
        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        TMP_Text preferredText = null;

        foreach (TMP_Text text in texts)
        {
            if (!IsHealthText(text))
            {
                continue;
            }

            text.text = healthTextValue;

            if (preferredText == null || (!preferredText.gameObject.activeInHierarchy && text.gameObject.activeInHierarchy))
            {
                preferredText = text;
            }
        }

        return preferredText;
    }

    private bool IsHealthText(TMP_Text text)
    {
        return text != null
            && (text == healthText
                || text.gameObject.name == "HP Text"
                || (!string.IsNullOrEmpty(text.text) && text.text.StartsWith("HP:")));
    }

    private void UpdateStatsUI()
    {
        if (statsUI != null)
        {
            statsUI.UpdateAllStats();
        }
    }
}
