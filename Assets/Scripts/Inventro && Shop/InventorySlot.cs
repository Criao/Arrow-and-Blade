using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSo itemSo;
    public int quantity;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;
    private InventoryManager inventoryManager;
    private static ShopManager activeShop;


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
    private void HandleShopStateChanged(ShopManager shopManager, bool isOpen)
    {
        activeShop = isOpen ? shopManager : null;
    }
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
