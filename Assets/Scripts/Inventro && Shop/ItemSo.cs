using UnityEngine;

/// <summary>
/// 物品数据ScriptableObject，定义物品的属性和效果
/// </summary>
[CreateAssetMenu(fileName = "New  Item")]
public class ItemSo : ScriptableObject
{
    [SerializeField] private string itemName; // 物品名称
    public string ItemName => itemName;
    [SerializeField][TextArea] private string itDescription; // 物品描述
    public string ItemDescription => itDescription;
    [SerializeField] private Sprite icon; // 物品图标
    public Sprite Icon => icon;
    public bool isGold; // 是否为金币
    [SerializeField] private int stackSize = 3; // 堆叠上限
    [SerializeField] private int sellPrice; // 售价
    public int SellPrice => sellPrice;
    public int StackSize => stackSize;

    [Header("Item Type")]
    public ItemType itemType = ItemType.Normal; // 物品类型

    [Header("Stats")]
    [SerializeField] private int currentHealth; // 恢复生命值
    public int CurrentHealth => currentHealth;
    [SerializeField] private int maxHealth; // 增加最大生命值
    public int MaxHealth => maxHealth;
    [SerializeField] private int speed; // 增加速度
    public int Speed => speed;
    [SerializeField] private int damage; // 增加伤害
    public int Damage => damage;

    [Header("For Temporary Items")]
    [SerializeField] private float duration; // 效果持续时间
    public float Duration => duration;
}

/// <summary>
/// 物品类型枚举
/// </summary>
public enum ItemType
{
    Normal,      // 普通物品（肉）
    Mushroom,    // 蘑菇（随机效果）
    Pumpkin      // 南瓜（速度提升）
}
