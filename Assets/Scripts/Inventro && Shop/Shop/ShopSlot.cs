using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Drawing;

/// <summary>
/// 商店槽位类，显示商品信息并处理购买交互
/// </summary>
public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemSo itemSo; // 商品物品数据
    public ItemSo ItemSo => itemSo;
    [SerializeField] private TMP_Text itemNameText; // 物品名称文本
    [SerializeField] private TMP_Text priceText; // 价格文本
    [SerializeField] private Image itemImage; // 物品图标
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopInfo shopInfo; // 商品信息面板
    [SerializeField] private int price; // 价格
    private int stock; // 库存
    public int Price => price;

    /// <summary>
    /// 初始化商店槽位（无库存版本）
    /// </summary>
    public void Initialize(ItemSo newItemSo, int price)
    {
        itemSo = newItemSo;
        itemImage.sprite = itemSo.Icon;
        itemNameText.text = itemSo.ItemName;
        this.price = price;
        priceText.text = price.ToString();
    }

    /// <summary>
    /// 购买按钮点击事件
    /// </summary>
    public void OnBuyButtonClicked()
    {
        if (shopManager == null)
        {
            Debug.LogError("❌ ShopSlot 的 shopManager 是 null！");
            return;
        }
        shopManager.TryBuyItem(itemSo, price);
    }

    /// <summary>
    /// 初始化商店槽位（带库存版本）
    /// </summary>
    public void Initialize(ItemSo newItemSo, int price, int initialStock)
    {
        itemSo = newItemSo;
        itemImage.sprite = itemSo.Icon;
        itemNameText.text = itemSo.ItemName;
        this.price = price;
        priceText.text = price.ToString();
        stock = initialStock;
    }

    /// <summary>
    /// 增加库存
    /// </summary>
    public void AddStock(int amount)
    {
        if (itemSo != null)
            stock += amount;
    }

    /// <summary>
    /// 鼠标进入时显示物品信息
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        shopInfo.ShowItemInfo(itemSo);
    }

    /// <summary>
    /// 鼠标离开时隐藏物品信息
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HandItemInfo();
    }

    /// <summary>
    /// 鼠标移动时更新信息面板位置
    /// </summary>
    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemSo != null)
            shopInfo.FollowMouse();
    }
}
