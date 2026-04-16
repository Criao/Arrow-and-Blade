using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class ShopManager : MonoBehaviour
{

    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private InventoryManager inventoryManager;


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
    public void TryBuyItem(ItemSo itemSo, int price)
    {
        // 先检查各个引用是否为空
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
    public void SellItem(ItemSo itemSo)
    {
        if (itemSo == null)
            return;
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
        // 不在商店里的物品，用 ItemSo 自带的售价
        inventoryManager.gold += itemSo.SellPrice;
        inventoryManager.goldText.text = inventoryManager.gold.ToString();
    }
    // ShopManager.cs 新增三个方法，把这三个绑给按钮
    public void OnItemTabClicked()
    {
        ShopKeeper.currentShopKeeper?.OpenItemShop();
    }

    public void OnWeaponTabClicked()
    {
        ShopKeeper.currentShopKeeper?.OpenWeaponShop();
    }

    public void OnArmourTabClicked()
    {
        ShopKeeper.currentShopKeeper?.OpenArmourShop();
    }
}

[Serializable]
public class ShopItem
{
    public ItemSo itemSo;
    public int price;
    public int initialStock = 5; // 初始库存
}
