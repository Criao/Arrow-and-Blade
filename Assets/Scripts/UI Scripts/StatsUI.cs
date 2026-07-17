using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private GameObject[] statsSlots;
    [SerializeField] private CanvasGroup statsCanvas;

    private string pauseOwner;
    private bool statsOpen;

    private void Awake()
    {
        pauseOwner = $"{nameof(StatsUI)}:{GetInstanceID()}";
    }

    private void Start()
    {
        SetCanvasVisible(false);
        UpdateAllStats();
    }

    private void Update()
    {
        if (!Input.GetButtonDown("ToggleStats") || statsCanvas == null)
        {
            return;
        }

        if (statsOpen)
        {
            CloseStatsPanel();
        }
        else if (PauseManager.IsPaused)
        {
            return;
        }
        else
        {
            OpenStatsPanel();
        }
    }

    private void OnDisable()
    {
        if (statsOpen)
        {
            statsOpen = false;
            PauseManager.SetPaused(pauseOwner, false);
        }
    }

    private void OpenStatsPanel()
    {
        statsOpen = true;
        SetCanvasVisible(true);
        PauseManager.SetPaused(pauseOwner, true);
    }

    private void CloseStatsPanel()
    {
        statsOpen = false;
        UpdateAllStats();
        SetCanvasVisible(false);
        PauseManager.SetPaused(pauseOwner, false);
    }

    private void SetCanvasVisible(bool visible)
    {
        if (statsCanvas == null)
        {
            return;
        }

        statsCanvas.alpha = visible ? 1f : 0f;
        statsCanvas.blocksRaycasts = visible;
        statsCanvas.interactable = visible;
    }

    public void UpdateDamage()
    {
        if (StatsManager.Instance == null || statsSlots == null || statsSlots.Length <= 0 || statsSlots[0] == null)
        {
            return;
        }

        TMP_Text text = statsSlots[0].GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = "Damage: " + StatsManager.Instance.Damage;
        }
    }

    private void UpdateSpeed()
    {
        if (StatsManager.Instance == null || statsSlots == null || statsSlots.Length <= 1 || statsSlots[1] == null)
        {
            return;
        }

        TMP_Text text = statsSlots[1].GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = "Speed: " + StatsManager.Instance.Speed;
        }
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
