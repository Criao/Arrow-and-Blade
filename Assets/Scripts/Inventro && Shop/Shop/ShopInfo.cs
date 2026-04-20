using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店物品信息面板，显示物品详细信息
/// </summary>
public class ShopInfo : MonoBehaviour
{
    [SerializeField] private CanvasGroup infoPanel; // 信息面板画布组
    [SerializeField] private TMP_Text itemNameText; // 物品名称
    [SerializeField] private TMP_Text itemDescriptionText; // 物品描述

    [Header("Stat Fields")]
    [SerializeField] private TMP_Text[] statTexts; // 属性文本数组

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
        infoPanel.alpha = 0;
        infoPanel.blocksRaycasts = false;
    }

    /// <summary>
    /// 显示物品信息
    /// </summary>
    public void ShowItemInfo(ItemSo itemSo)
    {
        infoPanel.alpha = 1;
        infoPanel.blocksRaycasts = true;
        itemNameText.text = itemSo.ItemName;
        itemDescriptionText.text = itemSo.ItemDescription;

        // 收集物品属性
        List<string> stats = new List<string>();
        if (itemSo.CurrentHealth > 0) stats.Add("Health:" + itemSo.CurrentHealth.ToString());
        if (itemSo.Damage > 0) stats.Add("Damage:" + itemSo.Damage.ToString());
        if (itemSo.Speed > 0) stats.Add("Speed:" + itemSo.Speed.ToString());
        if (itemSo.Duration > 0) stats.Add("Duration:" + itemSo.Duration.ToString());

        if (stats.Count <= 0)
            return;

        // 显示属性文本
        for (int i = 0; i < statTexts.Length; i++)
        {
            if (i < stats.Count)
            {
                statTexts[i].text = stats[i];
                statTexts[i].gameObject.SetActive(true);
            }
            else
            {
                statTexts[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 隐藏物品信息
    /// </summary>
    public void HandItemInfo()
    {
        infoPanel.alpha = 0;
        infoPanel.blocksRaycasts = false;
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    /// <summary>
    /// 让信息面板跟随鼠标
    /// </summary>
    public void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0);
        infoPanelRect.position = mousePosition + offset;
    }
}
