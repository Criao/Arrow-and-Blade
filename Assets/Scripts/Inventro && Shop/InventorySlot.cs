using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSo itemSo;
    public int quantity;

    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;

    private InventoryManager inventoryManager;
    private static ShopManager activeShop;

    private void Awake()
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
        if (isOpen)
        {
            activeShop = shopManager;
        }
        else if (activeShop == shopManager)
        {
            activeShop = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData == null || itemSo == null || quantity <= 0)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleRightClick();
        }
    }

    public void UpdateUI()
    {
        if (quantity <= 0)
        {
            quantity = 0;
            itemSo = null;
        }

        if (itemSo != null)
        {
            if (itemImage != null)
            {
                itemImage.sprite = itemSo.Icon;
                itemImage.preserveAspect = true;
                itemImage.gameObject.SetActive(itemSo.Icon != null);
            }

            if (quantityText != null)
            {
                quantityText.text = quantity.ToString();
            }
        }
        else
        {
            if (itemImage != null)
            {
                itemImage.sprite = null;
                itemImage.gameObject.SetActive(false);
            }

            if (quantityText != null)
            {
                quantityText.text = string.Empty;
            }
        }
    }

    private void HandleLeftClick()
    {
        if (inventoryManager == null)
        {
            inventoryManager = GetComponentInParent<InventoryManager>();
        }

        inventoryManager?.UseItem(this);
    }

    private void HandleRightClick()
    {
        if (activeShop != null)
        {
            SellCurrentItem();
            return;
        }

        if (inventoryManager == null)
        {
            inventoryManager = GetComponentInParent<InventoryManager>();
        }

        inventoryManager?.DropItem(this);
    }

    private void SellCurrentItem()
    {
        if (activeShop == null || !activeShop.SellItem(itemSo))
        {
            return;
        }

        quantity--;
        UpdateUI();
    }
}
