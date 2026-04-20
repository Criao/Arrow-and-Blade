using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using TMPro;

/// <summary>
/// 属性UI管理器，显示和更新玩家属性面板
/// </summary>
public class StatsUI : MonoBehaviour
{
    [SerializeField] private GameObject[] statsSlots; // 属性槽数组
    [SerializeField] private CanvasGroup statsCanvas; // 属性面板Canvas组
    private bool statsOpen = false; // 属性面板是否打开

    /// <summary>
    /// 初始化，隐藏属性面板并更新数据
    /// </summary>
    private void Start()
    {
        statsCanvas.alpha = 0;
        statsCanvas.blocksRaycasts = false;
        statsCanvas.interactable = false;
        UpdateAllStats();
    }

    /// <summary>
    /// 每帧检测输入，切换属性面板显示
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
        {
            if (statsOpen == true)
            {
                // 关闭属性面板
                Time.timeScale = 1;
                UpdateAllStats();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
                statsCanvas.interactable = false;
                statsOpen = false;
            }
            else
            {
                // 打开属性面板
                Time.timeScale = 0;
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                statsCanvas.interactable = true;
                statsOpen = true;
            }
        }
    }

    /// <summary>
    /// 更新伤害显示
    /// </summary>
    public void UpdateDamage()
    {
        if (statsSlots[0] != null)
        {
            TMP_Text text = statsSlots[0].GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = "Damage: " + StatsManager.Instance.Damage;
        }
    }

    /// <summary>
    /// 更新速度显示
    /// </summary>
    private void UpdateSpeed()
    {
        if (statsSlots[1] != null)
        {
            TMP_Text text = statsSlots[1].GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = "Speed: " + StatsManager.Instance.Speed;
        }
    }

    /// <summary>
    /// 更新所有属性显示
    /// </summary>
    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
