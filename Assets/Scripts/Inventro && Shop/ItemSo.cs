using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New  Item")]
public class ItemSo : ScriptableObject
{

    [SerializeField] private string itemName;
    public string ItemName => itemName;
    [SerializeField][TextArea] private string itDescription;
    public string ItemDescription => itDescription;
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;
    public bool isGold;
    [SerializeField] private int stackSize = 3;
    [SerializeField] private int sellPrice;
    public int SellPrice => sellPrice;
    public int StackSize => stackSize;

    [Header("Item Type")]
    public ItemType itemType = ItemType.Normal;

    [Header("Stats")]
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;
    [SerializeField] private int maxHealth;
    public int MaxHealth => maxHealth;
    [SerializeField] private int speed;
    public int Speed => speed;
    [SerializeField] private int damage;
    public int Damage => damage;

    [Header("For Temporary Items")]
    [SerializeField] private float duration;
    public float Duration => duration;
}

public enum ItemType
{
    Normal,      // 普通物品（肉）
    Mushroom,    // 蘑菇（随机效果）
    Pumpkin      // 南瓜（速度提升）
}
