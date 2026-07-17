using UnityEngine;

public class ToggleSkillTree : MonoBehaviour
{
    [SerializeField] private CanvasGroup statsCanvas;

    private string pauseOwner;
    private bool skillTreeOpen;

    private void Awake()
    {
        pauseOwner = $"{nameof(ToggleSkillTree)}:{GetInstanceID()}";
    }

    private void Start()
    {
        SetCanvasVisible(false);
    }

    private void Update()
    {
        if (!Input.GetButtonDown("ToggleSkillTree") || statsCanvas == null)
        {
            return;
        }

        if (skillTreeOpen)
        {
            CloseSkillTree();
        }
        else if (PauseManager.IsPaused)
        {
            return;
        }
        else
        {
            OpenSkillTree();
        }
    }

    private void OnDisable()
    {
        if (skillTreeOpen)
        {
            skillTreeOpen = false;
            PauseManager.SetPaused(pauseOwner, false);
        }
    }

    private void OpenSkillTree()
    {
        skillTreeOpen = true;
        SetCanvasVisible(true);
        PauseManager.SetPaused(pauseOwner, true);
    }

    private void CloseSkillTree()
    {
        skillTreeOpen = false;
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
}
