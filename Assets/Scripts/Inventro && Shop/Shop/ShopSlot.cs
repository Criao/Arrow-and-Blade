using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Drawing;


public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemSo itemSo;
    public ItemSo ItemSo => itemSo;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image itemImage;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopInfo shopInfo;
    [SerializeField] private int price;
    private int stock;
    public int Price => price;

    public void Initialize(ItemSo newItemSo, int price)
    {
        itemSo = newItemSo;
        itemImage.sprite = itemSo.Icon;
        itemNameText.text = itemSo.ItemName;
        this.price = price;
        priceText.text = price.ToString();
    }
    public void OnBuyButtonClicked()
    {
        if (shopManager == null)
        {
            Debug.LogError("❌ ShopSlot 的 shopManager 是 null！");
            return;
        }
        shopManager.TryBuyItem(itemSo, price);
    }



    public void Initialize(ItemSo newItemSo, int price, int initialStock)
    {
        itemSo = newItemSo;
        itemImage.sprite = itemSo.Icon;
        itemNameText.text = itemSo.ItemName;
        this.price = price;
        priceText.text = price.ToString();
        stock = initialStock;
    }

    public void AddStock(int amount)
    {
        if (itemSo != null)
            stock += amount;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        shopInfo.ShowItemInfo(itemSo);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HandItemInfo();
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemSo != null)
            shopInfo.FollowMouse();
    }
}
