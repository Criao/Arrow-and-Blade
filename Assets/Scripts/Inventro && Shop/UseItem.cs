using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void ApplyItemEffects(ItemSo itemSo)
    {
        Debug.Log($"使用物品：{itemSo.ItemName}，类型：{itemSo.itemType}");

        // 根据物品类型应用不同效果
        switch (itemSo.itemType)
        {
            case ItemType.Normal:
                Debug.Log("执行普通物品效果");
                ApplyNormalItem(itemSo);
                break;
            case ItemType.Mushroom:
                Debug.Log("执行蘑菇效果");
                ApplyMushroomEffect();
                break;
            case ItemType.Pumpkin:
                Debug.Log("执行南瓜效果");
                ApplyPumpkinEffect();
                break;
        }
    }

    // 普通物品效果（肉）
    private void ApplyNormalItem(ItemSo itemSo)
    {
        if (itemSo.CurrentHealth > 0)
            StatsManager.Instance.UpdateHealth(itemSo.CurrentHealth);
        if (itemSo.MaxHealth > 0)
            StatsManager.Instance.UpdateMaxHealth(itemSo.MaxHealth);
        if (itemSo.Speed > 0)
            StatsManager.Instance.UpdateSpeed(itemSo.Speed);
        if (itemSo.Duration > 0)
            StartCoroutine(EffectTimer(itemSo, itemSo.Duration));
    }

    // 蘑菇效果：50%概率掉1滴血 或 增加攻击力1点30秒
    private void ApplyMushroomEffect()
    {
        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
        {
            // 50%概率：掉1滴血
            Debug.Log("蘑菇效果：掉1滴血");
            StatsManager.Instance.UpdateHealth(-1);
        }
        else
        {
            // 50%概率：增加攻击力1点持续30秒
            Debug.Log("蘑菇效果：增加攻击力1点持续30秒");
            StatsManager.Instance.UpdateDamage(1);
            StartCoroutine(RemoveDamageBonus(1, 30f));
        }
    }

    // 南瓜效果：提高移动速度20秒
    private void ApplyPumpkinEffect()
    {
        Debug.Log("南瓜效果：提高移动速度20秒");
        int speedBonus = 2; // 速度提升量
        StatsManager.Instance.UpdateSpeed(speedBonus);
        StartCoroutine(RemoveSpeedBonus(speedBonus, 20f));
    }

    // 移除攻击力加成
    private IEnumerator RemoveDamageBonus(int amount, float duration)
    {
        yield return new WaitForSecondsRealtime(duration); // 使用Realtime，不受Time.timeScale影响
        StatsManager.Instance.UpdateDamage(-amount);
        Debug.Log("蘑菇效果结束：攻击力恢复");
    }

    // 移除速度加成
    private IEnumerator RemoveSpeedBonus(int amount, float duration)
    {
        yield return new WaitForSecondsRealtime(duration); // 使用Realtime，不受Time.timeScale影响
        StatsManager.Instance.UpdateSpeed(-amount);
        Debug.Log("南瓜效果结束：速度恢复");
    }

    private IEnumerator EffectTimer(ItemSo itemSo, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (itemSo.CurrentHealth > 0)
            StatsManager.Instance.UpdateHealth(-itemSo.CurrentHealth);
        if (itemSo.MaxHealth > 0)
            StatsManager.Instance.UpdateMaxHealth(-itemSo.MaxHealth);
        if (itemSo.Speed > 0)
            StatsManager.Instance.UpdateSpeed(-itemSo.Speed);
    }
}
