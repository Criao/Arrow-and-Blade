using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

/// <summary>
/// 背包管理器，处理物品的添加、使用、丢弃和金币管理
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] itemSlots; // 背包槽位数组
    [SerializeField] private UseItem useItem; // 物品使用组件
    public int gold; // 金币数量
    public TMP_Text goldText; // 金币显示文本
    [SerializeField] private GameObject lootPrefab; // 掉落物预制体
    [SerializeField] private Transform player; // 玩家Transform

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        goldText.text = gold.ToString();
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    /// <summary>
    /// 添加物品到背包
    /// </summary>
    public void AddItem(ItemSo itemSo, int quantity)
    {
        // 如果是金币，直接添加金币
        if (itemSo.isGold)
        {
            AddGold(quantity);
            return;
        }

        // 先尝试堆叠到已有物品
        foreach (var slot in itemSlots)
        {
            if (slot.itemSo == itemSo && slot.quantity < itemSo.StackSize)
            {
                int availableSpace = itemSo.StackSize - slot.quantity;
                int amountToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                slot.UpdateUI();

                if (quantity <= 0)
                    return;
            }
        }

        // 如果还有剩余，放入空槽位
        foreach (var slot in itemSlots)
        {
            if (slot.itemSo == null)
            {
                int amountToAdd = Mathf.Min(itemSo.StackSize,quantity);
                slot.itemSo = itemSo;
                slot.quantity = quantity;
                slot.UpdateUI();
                return;
            }
        }

        // 如果背包满了，掉落到地上
        if(quantity > 0)
        {
            DropLoot(itemSo,quantity);
        }
    }

    /// <summary>
    /// 添加金币
    /// </summary>
    public void AddGold(int amount)
    {
        gold += amount;
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
        Debug.Log($"金币增加 {amount}，当前金币: {gold}");
    }

    /// <summary>
    /// 丢弃物品
    /// </summary>
    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSo,1);
        slot.quantity--;
        if(slot.quantity <= 0)
        {
            slot.itemSo = null;
        }
        slot.UpdateUI();
    }

    /// <summary>
    /// 在玩家位置生成掉落物
    /// </summary>
    private void DropLoot(ItemSo itemSo,int quantity)
    {
        Loot loot = Instantiate(lootPrefab,player.position,Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSo,quantity);
    }

    /// <summary>
    /// 使用物品
    /// </summary>
    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSo != null && slot.quantity > 0)
        {
            useItem.ApplyItemEffects(slot.itemSo);
            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSo = null;
            }
            slot.UpdateUI();
        }
    }
}
