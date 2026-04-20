using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 商店管理员类，处理商店的开关和玩家交互
/// </summary>
public class ShopKeeper : MonoBehaviour
{
    public static event Action<ShopManager, bool> OnShopStateChanged; // 商店状态变化事件
    public static ShopKeeper currentShopKeeper; // 当前激活的商店管理员
    [SerializeField] private Animator anim;
    [SerializeField] private CanvasGroup shopCanvasGroup; // 商店UI画布组
    [SerializeField] private ShopManager shopManager;

    [SerializeField] private List<ShopItem> shopItems; // 物品商店列表
    [SerializeField] private List<ShopItem> shopWeapons; // 武器商店列表
    [SerializeField] private List<ShopItem> shopArmour; // 护甲商店列表
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -10);
    private bool playerInRange; // 玩家是否在范围内
    private bool isShopOpen; // 商店是否打开

    private void Update()
    {
        if (playerInRange)
        {
            // 按下交互键打开/关闭商店
            if (Input.GetButtonDown("Interact"))
            {
                if (!isShopOpen)
                {
                    Time.timeScale = 0; // 暂停游戏
                    currentShopKeeper = this;
                    isShopOpen = true;
                    OnShopStateChanged?.Invoke(shopManager, true);
                    shopCanvasGroup.alpha = 1;
                    shopCanvasGroup.blocksRaycasts = true;
                    shopCanvasGroup.interactable = true;

                    OpenItemShop();
                }
                else
                {
                    Time.timeScale = 1; // 恢复游戏
                    currentShopKeeper = null;
                    isShopOpen = false;
                    OnShopStateChanged?.Invoke(shopManager, false);
                    shopCanvasGroup.alpha = 0;
                    shopCanvasGroup.blocksRaycasts = false;
                    shopCanvasGroup.interactable = false;
                }
            }
        }
    }

    /// <summary>
    /// 打开物品商店
    /// </summary>
    public void OpenItemShop()
    {
        shopManager.PopulateShopItems(shopItems);
    }

    /// <summary>
    /// 打开武器商店
    /// </summary>
    public void OpenWeaponShop()
    {
        shopManager.PopulateShopItems(shopWeapons);
    }

    /// <summary>
    /// 打开护甲商店
    /// </summary>
    public void OpenArmourShop()
    {
        shopManager.PopulateShopItems(shopArmour);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetBool("PlayerInRange", true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetBool("PlayerInRange", false);
            playerInRange = false;
        }
    }
}
