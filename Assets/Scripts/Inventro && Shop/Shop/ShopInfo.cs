using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopInfo : MonoBehaviour
{
    [SerializeField] private CanvasGroup infoPanel;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Stat Fields")]
    [SerializeField] private TMP_Text[] statTexts;

    private RectTransform infoPanelRect;


    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
        infoPanel.alpha = 0;
        infoPanel.blocksRaycasts = false;
    }

    public void ShowItemInfo(ItemSo itemSo)
    {
        infoPanel.alpha = 1;
        infoPanel.blocksRaycasts = true;
        itemNameText.text = itemSo.ItemName;
        itemDescriptionText.text = itemSo.ItemDescription;

        List<string> stats = new List<string>();
        if (itemSo.CurrentHealth > 0) stats.Add("Health:" + itemSo.CurrentHealth.ToString());
        if (itemSo.Damage > 0) stats.Add("Damage:" + itemSo.Damage.ToString());
        if (itemSo.Speed > 0) stats.Add("Speed:" + itemSo.Speed.ToString());
        if (itemSo.Duration > 0) stats.Add("Duration:" + itemSo.Duration.ToString());

        if (stats.Count <= 0)
            return;

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
    public void HandItemInfo()
    {
        infoPanel.alpha = 0;
        infoPanel.blocksRaycasts = false;
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }
    public void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0);
        infoPanelRect.position = mousePosition + offset;
    }


}
