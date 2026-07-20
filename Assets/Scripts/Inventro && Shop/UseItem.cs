using System.Collections;
using UnityEngine;

/// <summary>
/// 物品使用类，处理不同类型物品的效果应用
/// </summary>
public class UseItem : MonoBehaviour
{
    /// <summary>
    /// 应用物品效果
    /// </summary>
    public void ApplyItemEffects(ItemSo itemSo)
    {
        if (itemSo == null || StatsManager.Instance == null)
        {
            Debug.LogWarning("Cannot apply item effects because item or StatsManager is missing.");
            return;
        }


        // 根据物品类型应用不同效果
        switch (itemSo.itemType)
        {
            case ItemType.Normal:
                ApplyNormalItem(itemSo);
                break;
            case ItemType.Mushroom:
                ApplyMushroomEffect();
                break;
            case ItemType.Pumpkin:
                ApplyPumpkinEffect();
                break;
        }
    }

    /// <summary>
    /// 普通物品效果（如肉类）
    /// </summary>
    private void ApplyNormalItem(ItemSo itemSo)
    {
        StatsManager stats = StatsManager.Instance;
        if (stats == null)
        {
            return;
        }

        if (itemSo.CurrentHealth > 0)
            stats.UpdateHealth(itemSo.CurrentHealth);
        if (itemSo.MaxHealth > 0)
            stats.UpdateMaxHealth(itemSo.MaxHealth);
        if (itemSo.Speed > 0)
            stats.UpdateSpeed(itemSo.Speed);
        if (itemSo.Damage > 0)
            stats.UpdateDamage(itemSo.Damage);
        if (itemSo.Duration > 0 && (itemSo.MaxHealth > 0 || itemSo.Speed > 0 || itemSo.Damage > 0))
            StartCoroutine(EffectTimer(itemSo, itemSo.Duration));
    }

    /// <summary>
    /// 蘑菇效果：50%概率掉1滴血 或 增加攻击力1点30秒
    /// </summary>
    private void ApplyMushroomEffect()
    {
        StatsManager stats = StatsManager.Instance;
        if (stats == null)
        {
            return;
        }

        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
        {
            stats.UpdateHealth(-1);
        }
        else
        {
            stats.UpdateDamage(1);
            StartCoroutine(RemoveDamageBonus(1, 30f));
        }
    }

    /// <summary>
    /// 南瓜效果：提高移动速度20秒
    /// </summary>
    private void ApplyPumpkinEffect()
    {
        StatsManager stats = StatsManager.Instance;
        if (stats == null)
        {
            return;
        }

        int speedBonus = 2;
        stats.UpdateSpeed(speedBonus);
        StartCoroutine(RemoveSpeedBonus(speedBonus, 20f));
    }

    /// <summary>
    /// 移除攻击力加成
    /// </summary>
    private IEnumerator RemoveDamageBonus(int amount, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        if (StatsManager.Instance == null)
        {
            yield break;
        }

        StatsManager.Instance.UpdateDamage(-amount);
    }

    /// <summary>
    /// 移除速度加成
    /// </summary>
    private IEnumerator RemoveSpeedBonus(int amount, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        if (StatsManager.Instance == null)
        {
            yield break;
        }

        StatsManager.Instance.UpdateSpeed(-amount);
    }

    /// <summary>
    /// 临时效果计时器
    /// </summary>
    private IEnumerator EffectTimer(ItemSo itemSo, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        StatsManager stats = StatsManager.Instance;
        if (stats == null)
        {
            yield break;
        }

        if (itemSo.MaxHealth > 0)
            stats.UpdateMaxHealth(-itemSo.MaxHealth);
        if (itemSo.Speed > 0)
            stats.UpdateSpeed(-itemSo.Speed);
        if (itemSo.Damage > 0)
            stats.UpdateDamage(-itemSo.Damage);
    }
}
