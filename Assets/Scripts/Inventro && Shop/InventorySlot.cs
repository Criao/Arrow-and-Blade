using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

/// <summary>
/// 背包槽位类，处理物品的显示和交互（使用、出售、丢弃）
/// </summary>
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSo itemSo; // 槽位中的物品
    public int quantity; // 物品数量
    [SerializeField] private Image itemImage; // 物品图标
    [SerializeField] private TMP_Text quantityText; // 数量文本
    private InventoryManager inventoryManager;
    private static ShopManager activeShop; // 当前打开的商店

    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopStateChanged += HandleShopStateChanged;
    }

    private void OnDisable()
    {
        ShopKeeper.OnShopStateChanged -= HandleShopStateChanged;
    }

    /// <summary>
    /// 处理商店状态变化
    /// </summary>
    private void HandleShopStateChanged(ShopManager shopManager, bool isOpen)
    {
        activeShop = isOpen ? shopManager : null;
    }

    /// <summary>
    /// 处理鼠标点击事件
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity > 0)
        {
            // 左键 - 使用或出售
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("activeShop = " + activeShop);
                if (activeShop != null)
                {
                    Debug.Log("走售出分支");
                    activeShop.SellItem(itemSo);
                    quantity--;
                    UpdateUI();
                }
                else
                {
                    Debug.Log("走使用分支");
                    inventoryManager.UseItem(this);
                }
            }
            // 右键 - 丢弃物品
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("右键点击 - 丢弃物品");
                inventoryManager.DropItem(this);
            }
        }
    }

    /// <summary>
    /// 更新槽位UI显示
    /// </summary>
    public void UpdateUI()
    {
        if (quantity <= 0)
            itemSo = null;

        if (itemSo != null)
        {
            itemImage.sprite = itemSo.Icon;
            itemImage.preserveAspect = true;
            itemImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }
}
