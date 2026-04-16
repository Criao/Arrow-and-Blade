using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private GameObject[] statsSlots;
    [SerializeField] private CanvasGroup statsCanvas;
    private bool statsOpen = false;
    private void Start()
    {
        statsCanvas.alpha = 0;             
        statsCanvas.blocksRaycasts = false;
        statsCanvas.interactable = false;
        UpdateAllStats();
    }
    private void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
        {
            if (statsOpen == true)
            {
                Time.timeScale = 1;
                UpdateAllStats();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
                statsCanvas.interactable = false;
                statsOpen = false;
            }
            else
            {
                Time.timeScale = 0;
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                statsCanvas.interactable = true;
                statsOpen = true;
            }


        }
    }
    public void UpdateDamage()
    {
        if (statsSlots[0] != null)
        {
            TMP_Text text = statsSlots[0].GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = "Damage: " + StatsManager.Instance.Damage;
        }
    }
    private void UpdateSpeed()
    {
        if (statsSlots[1] != null)
        {
            TMP_Text text = statsSlots[1].GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = "Speed: " + StatsManager.Instance.Speed;
        }
    }
    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
