using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Animator healthTextAnimator;

    private void Start()
    {
        UpdateHealthUI();
        GameOverManager.EnsureInstance().RegisterPlayer(gameObject);
    }

    public void ChangeHealth(int amount)
    {
        ApplyHealthChange(amount);

        if (healthTextAnimator != null)
        {
            healthTextAnimator.Play("TextUpdate");
        }

        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (healthText == null || StatsManager.Instance == null)
        {
            return;
        }

        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
    }

    private void ApplyHealthChange(int amount)
    {
        if (StatsManager.Instance == null)
        {
            Debug.LogError("StatsManager.Instance is missing. Cannot change player health.");
            return;
        }

        StatsManager.Instance.CurrentHealth += amount;
        Died();
    }

    private void Died()
    {
        if (StatsManager.Instance == null || StatsManager.Instance.CurrentHealth > 0)
        {
            return;
        }

        StatsManager.Instance.CurrentHealth = 0;
        Debug.Log("Player died. Showing Game Over.");

        GameOverManager gameOverManager = GameOverManager.EnsureInstance();
        gameOverManager.ShowGameOver();

        gameObject.SetActive(false);
    }
}
