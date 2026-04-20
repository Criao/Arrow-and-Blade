using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

/// <summary>
/// 商店管理器，处理商品展示、购买和出售逻辑
/// </summary>
public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots; // 商店槽位数组
    [SerializeField] private InventoryManager inventoryManager;

    /// <summary>
    /// 填充商店物品
    /// </summary>
    public void PopulateShopItems(List<ShopItem> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItem shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSo, shopItem.price, shopItem.initialStock);
            shopSlots[i].gameObject.SetActive(true);
        }
        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 尝试购买物品
    /// </summary>
    public void TryBuyItem(ItemSo itemSo, int price)
    {
        if (inventoryManager == null)
        {
            Debug.LogError("❌ inventoryManager 是 null！");
            return;
        }
        if (itemSo == null)
        {
            Debug.LogError("❌ itemSo 是 null！");
            return;
        }

        Debug.Log($"尝试购买: {itemSo.ItemName}, 价格:{price}, 当前金币:{inventoryManager.gold}");

        if (inventoryManager.gold < price)
        {
            Debug.Log("❌ 金币不足");
            return;
        }

        if (!HasSpaceForItem(itemSo))
        {
            Debug.Log("❌ 背包已满");
            return;
        }

        inventoryManager.gold -= price;
        inventoryManager.goldText.text = inventoryManager.gold.ToString();
        inventoryManager.AddItem(itemSo, 1);
        Debug.Log($"✅ 购买成功: {itemSo.ItemName}");
    }

    /// <summary>
    /// 检查背包是否有空间
    /// </summary>
    private bool HasSpaceForItem(ItemSo itemSo)
    {
        foreach (var slot in inventoryManager.itemSlots)
        {
            if (slot.itemSo == itemSo && slot.quantity < itemSo.StackSize)
                return true;
            else if (slot.itemSo == null)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 出售物品
    /// </summary>
    public void SellItem(ItemSo itemSo)
    {
        if (itemSo == null)
            return;

        // 如果物品在商店中，按商店价格-1出售
        foreach (var slot in shopSlots)
        {
            if (slot.ItemSo == itemSo)
            {
                inventoryManager.gold += slot.Price - 1;
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                slot.AddStock(1);
                return;
            }
        }

        // 不在商店里的物品，用ItemSo自带的售价
        inventoryManager.gold += itemSo.SellPrice;
        inventoryManager.goldText.text = inventoryManager.gold.ToString();
    }

    /// <summary>
    /// 物品标签页点击事件
    /// </summary>
    public void OnItemTabClicked()
    {
        ShopKeeper.currentShopKeeper?.OpenItemShop();
    }

    /// <summary>
    /// 武器标签页点击事件
    /// </summary>
    public void OnWeaponTabClicked()
    {
        ShopKeeper.currentShopKeeper?.OpenWeaponShop();
    }

    /// <summary>
    /// 护甲标签页点击事件
    /// </summary>
    public void OnArmourTabClicked()
    {
        ShopKeeper.currentShopKeeper?.OpenArmourShop();
    }
}

/// <summary>
/// 商店物品数据类
/// </summary>
[Serializable]
public class ShopItem
{
    public ItemSo itemSo; // 物品数据
    public int price; // 价格
    public int initialStock = 5; // 初始库存
}
