using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private InventoryManager inventoryManager;

    public void PopulateShopItems(List<ShopItem> shopItems)
    {
        if (shopSlots == null)
        {
            Debug.LogWarning($"{nameof(ShopManager)} has no shop slots assigned.");
            return;
        }

        int slotIndex = 0;

        if (shopItems != null)
        {
            for (int i = 0; i < shopItems.Count && slotIndex < shopSlots.Length; i++, slotIndex++)
            {
                ShopSlot slot = shopSlots[slotIndex];
                if (slot == null)
                {
                    continue;
                }

                ShopItem shopItem = shopItems[i];
                if (shopItem == null || shopItem.itemSo == null)
                {
                    slot.Clear();
                    slot.gameObject.SetActive(false);
                    continue;
                }

                int initialStock = shopItem.initialStock > 0 ? shopItem.initialStock : int.MaxValue;
                slot.Initialize(shopItem.itemSo, Mathf.Max(0, shopItem.price), initialStock);
                slot.gameObject.SetActive(true);
            }
        }

        for (; slotIndex < shopSlots.Length; slotIndex++)
        {
            ShopSlot slot = shopSlots[slotIndex];
            if (slot == null)
            {
                continue;
            }

            slot.Clear();
            slot.gameObject.SetActive(false);
        }
    }

    public bool TryBuyItem(ItemSo itemSo, int price)
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning($"{nameof(ShopManager)} cannot buy because inventoryManager is missing.");
            return false;
        }

        if (itemSo == null)
        {
            Debug.LogWarning($"{nameof(ShopManager)} cannot buy a missing item.");
            return false;
        }

        int safePrice = Mathf.Max(0, price);
        if (inventoryManager.gold < safePrice)
        {
            Debug.Log("Not enough gold.");
            return false;
        }

        if (!HasSpaceForItem(itemSo))
        {
            Debug.Log("Inventory is full.");
            return false;
        }

        inventoryManager.AddGold(-safePrice);
        inventoryManager.AddItem(itemSo, 1);
        return true;
    }

    private bool HasSpaceForItem(ItemSo itemSo)
    {
        if (inventoryManager == null || inventoryManager.itemSlots == null || itemSo == null)
        {
            return false;
        }

        int stackSize = Mathf.Max(1, itemSo.StackSize);
        foreach (InventorySlot slot in inventoryManager.itemSlots)
        {
            if (slot == null)
            {
                continue;
            }

            if (slot.itemSo == itemSo && slot.quantity < stackSize)
            {
                return true;
            }

            if (slot.itemSo == null)
            {
                return true;
            }
        }

        return false;
    }

    public bool SellItem(ItemSo itemSo)
    {
        if (inventoryManager == null || itemSo == null)
        {
            return false;
        }

        if (shopSlots != null)
        {
            foreach (ShopSlot slot in shopSlots)
            {
                if (slot == null || slot.ItemSo != itemSo)
                {
                    continue;
                }

                inventoryManager.AddGold(Mathf.Max(0, slot.Price - 1));
                slot.AddStock(1);
                return true;
            }
        }

        inventoryManager.AddGold(Mathf.Max(0, itemSo.SellPrice));
        return true;
    }

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
    public int initialStock = 5;
}
