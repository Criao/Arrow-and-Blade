using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店按钮切换类，处理商店标签页切换
/// </summary>
public class ShopButtonToggle : MonoBehaviour
{
    /// <summary>
    /// 打开物品商店
    /// </summary>
    public void OpenItemShop()
    {
        if(ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenItemShop();
        }
    }

    /// <summary>
    /// 打开武器商店
    /// </summary>
    public void OpenWeaponShop()
    {
        if(ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenWeaponShop();
        }
    }

    /// <summary>
    /// 打开护甲商店
    /// </summary>
    public void OpenArmourShop()
    {
        if(ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenArmourShop();
        }
    }
}
