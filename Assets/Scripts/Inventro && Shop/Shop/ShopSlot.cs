using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
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
        Initialize(newItemSo, price, int.MaxValue);
    }

    public void Initialize(ItemSo newItemSo, int price, int initialStock)
    {
        if (newItemSo == null)
        {
            Clear();
            return;
        }

        itemSo = newItemSo;
        this.price = Mathf.Max(0, price);
        stock = Mathf.Max(0, initialStock);

        if (itemImage != null)
        {
            itemImage.sprite = itemSo.Icon;
            itemImage.enabled = itemSo.Icon != null;
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemSo.ItemName;
        }

        if (priceText != null)
        {
            priceText.text = this.price.ToString();
        }
    }

    public void Clear()
    {
        itemSo = null;
        price = 0;
        stock = 0;

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }

        if (itemNameText != null)
        {
            itemNameText.text = string.Empty;
        }

        if (priceText != null)
        {
            priceText.text = string.Empty;
        }
    }

    public void OnBuyButtonClicked()
    {
        if (shopManager == null || itemSo == null)
        {
            return;
        }

        if (stock <= 0)
        {
            Debug.Log("Item is out of stock.");
            return;
        }

        if (!shopManager.TryBuyItem(itemSo, price))
        {
            return;
        }

        if (stock != int.MaxValue)
        {
            stock--;
            if (stock <= 0)
            {
                if (shopInfo != null)
                {
                    shopInfo.HandItemInfo();
                }

                gameObject.SetActive(false);
            }
        }
    }

    public void AddStock(int amount)
    {
        if (itemSo == null || amount <= 0)
        {
            return;
        }

        if (stock != int.MaxValue)
        {
            stock += amount;
            if (stock > 0)
            {
                gameObject.SetActive(true);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopInfo != null && itemSo != null)
        {
            shopInfo.ShowItemInfo(itemSo);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (shopInfo != null)
        {
            shopInfo.HandItemInfo();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (shopInfo != null && itemSo != null)
        {
            shopInfo.FollowMouse();
        }
    }
}
