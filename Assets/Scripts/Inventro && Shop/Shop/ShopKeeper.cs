using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static event Action<ShopManager, bool> OnShopStateChanged;
    public static ShopKeeper currentShopKeeper;

    [SerializeField] private Animator anim;
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private ShopManager shopManager;

    [SerializeField] private List<ShopItem> shopItems;
    [SerializeField] private List<ShopItem> shopWeapons;
    [SerializeField] private List<ShopItem> shopArmour;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -10);

    private string pauseOwner;
    private bool playerInRange;
    private bool isShopOpen;

    private void Awake()
    {
        pauseOwner = $"{nameof(ShopKeeper)}:{GetInstanceID()}";
    }

    private void Start()
    {
        SetShopCanvasVisible(false);
    }

    private void Update()
    {
        if (!playerInRange || !Input.GetButtonDown("Interact"))
        {
            return;
        }

        if (isShopOpen)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    private void OnDisable()
    {
        CloseShop();
    }

    public void OpenItemShop()
    {
        PopulateShop(shopItems);
    }

    public void OpenWeaponShop()
    {
        PopulateShop(shopWeapons);
    }

    public void OpenArmourShop()
    {
        PopulateShop(shopArmour);
    }

    private void OpenShop()
    {
        if (shopManager == null)
        {
            Debug.LogWarning($"{nameof(ShopKeeper)} cannot open because shopManager is missing.");
            return;
        }

        currentShopKeeper = this;
        isShopOpen = true;
        OnShopStateChanged?.Invoke(shopManager, true);
        SetShopCanvasVisible(true);
        PauseManager.SetPaused(pauseOwner, true);
        OpenItemShop();
    }

    private void CloseShop()
    {
        bool wasOpen = isShopOpen;

        if (currentShopKeeper == this)
        {
            currentShopKeeper = null;
        }

        isShopOpen = false;
        SetShopCanvasVisible(false);
        PauseManager.SetPaused(pauseOwner, false);

        if (wasOpen)
        {
            OnShopStateChanged?.Invoke(shopManager, false);
        }
    }

    private void PopulateShop(List<ShopItem> items)
    {
        if (shopManager == null)
        {
            Debug.LogWarning($"{nameof(ShopKeeper)} cannot populate shop because shopManager is missing.");
            return;
        }

        shopManager.PopulateShopItems(items);
    }

    private void SetShopCanvasVisible(bool visible)
    {
        if (shopCanvasGroup == null)
        {
            return;
        }

        shopCanvasGroup.alpha = visible ? 1f : 0f;
        shopCanvasGroup.blocksRaycasts = visible;
        shopCanvasGroup.interactable = visible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (anim != null)
        {
            anim.SetBool("PlayerInRange", true);
        }

        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (anim != null)
        {
            anim.SetBool("PlayerInRange", false);
        }

        playerInRange = false;
        CloseShop();
    }
}
