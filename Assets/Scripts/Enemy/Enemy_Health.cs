using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private int expReward = 2;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeateds;

    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }

        if (currentHealth > 0)
        {
            return;
        }

        Die();
    }

    private void Die()
    {
        isDead = true;

        OnMonsterDefeateds?.Invoke(expReward);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateKillProgress(gameObject.tag);
        }

        Destroy(gameObject);
    }
}
