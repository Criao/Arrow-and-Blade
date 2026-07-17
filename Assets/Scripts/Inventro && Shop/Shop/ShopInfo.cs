using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        infoPanelRect = infoPanel != null
            ? infoPanel.transform as RectTransform
            : transform as RectTransform;

        HandItemInfo();
    }

    public void ShowItemInfo(ItemSo itemSo)
    {
        if (itemSo == null || infoPanel == null)
        {
            HandItemInfo();
            return;
        }

        infoPanel.alpha = 1f;
        infoPanel.blocksRaycasts = true;

        if (itemNameText != null)
        {
            itemNameText.text = itemSo.ItemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = itemSo.ItemDescription;
        }

        List<string> stats = new List<string>();
        if (itemSo.CurrentHealth > 0) stats.Add("Health: " + itemSo.CurrentHealth);
        if (itemSo.Damage > 0) stats.Add("Damage: " + itemSo.Damage);
        if (itemSo.Speed > 0) stats.Add("Speed: " + itemSo.Speed);
        if (itemSo.Duration > 0) stats.Add("Duration: " + itemSo.Duration);

        UpdateStatTexts(stats);
    }

    public void HandItemInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.alpha = 0f;
            infoPanel.blocksRaycasts = false;
        }

        if (itemNameText != null)
        {
            itemNameText.text = string.Empty;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = string.Empty;
        }

        UpdateStatTexts(null);
    }

    public void FollowMouse()
    {
        if (infoPanelRect == null)
        {
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(10f, -10f, 0f);
        infoPanelRect.position = mousePosition + offset;
    }

    private void UpdateStatTexts(List<string> stats)
    {
        if (statTexts == null)
        {
            return;
        }

        int count = stats != null ? stats.Count : 0;
        for (int i = 0; i < statTexts.Length; i++)
        {
            TMP_Text statText = statTexts[i];
            if (statText == null)
            {
                continue;
            }

            bool hasValue = i < count;
            statText.text = hasValue ? stats[i] : string.Empty;
            statText.gameObject.SetActive(hasValue);
        }
    }
}
