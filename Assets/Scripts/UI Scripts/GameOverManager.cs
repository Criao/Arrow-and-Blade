using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public Button continueButton;
    public Button quitButton;

    [Header("Continue Settings")]
    public bool usePlayerStartPosition = true;
    public Vector3 respawnPosition;
    public int respawnHealthPercent = 50;

    private GameObject player;
    private Vector3 playerStartPosition;
    private bool hasPlayerStartPosition;

    public static GameOverManager EnsureInstance()
    {
        if (Instance != null)
        {
            Instance.EnsureReady();
            return Instance;
        }

        Instance = FindLoadedManager();
        if (Instance == null)
        {
            GameObject managerObject = new GameObject(nameof(GameOverManager));
            Instance = managerObject.AddComponent<GameOverManager>();
        }

        Instance.EnsureReady();
        return Instance;
    }

    private static GameOverManager FindLoadedManager()
    {
        GameOverManager[] managers = Resources.FindObjectsOfTypeAll<GameOverManager>();
        for (int i = 0; i < managers.Length; i++)
        {
            GameOverManager manager = managers[i];
            if (manager == null)
            {
                continue;
            }

            Scene scene = manager.gameObject.scene;
            if (scene.IsValid() && scene.isLoaded)
            {
                return manager;
            }
        }

        return null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureReady();
    }

    private void Start()
    {
        EnsureReady();
        HideGameOverPanel();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterPlayer(GameObject playerObject)
    {
        if (playerObject == null)
        {
            return;
        }

        player = playerObject;
        RecordPlayerStartPosition();
    }

    public void ShowGameOver()
    {
        EnsureReady();

        if (gameOverPanel == null)
        {
            Debug.LogError("GameOver panel is missing and could not be created.");
            return;
        }

        ActivateHierarchy(gameOverPanel.transform);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SetRespawnPosition(Vector3 position)
    {
        respawnPosition = position;
        hasPlayerStartPosition = false;
    }

    private void EnsureReady()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (!enabled)
        {
            enabled = true;
        }

        EnsurePlayerReference();
        EnsureGameOverPanel();
        EnsureEventSystem();
        BindButtonEvents();
    }

    private void EnsurePlayerReference()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            RecordPlayerStartPosition();
        }
    }

    private void RecordPlayerStartPosition()
    {
        if (!usePlayerStartPosition || hasPlayerStartPosition || player == null)
        {
            return;
        }

        playerStartPosition = player.transform.position;
        hasPlayerStartPosition = true;
    }

    private void EnsureGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            return;
        }

        CreateDefaultGameOverUI();
    }

    private void CreateDefaultGameOverUI()
    {
        GameObject canvasObject = new GameObject("GameOverCanvas");
        MoveToManagerScene(canvasObject);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        gameOverPanel = CreateUIObject("GameOverPanel", canvasObject.transform, typeof(Image));
        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = gameOverPanel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.82f);

        gameOverText = CreateText(
            "GameOverText",
            gameOverPanel.transform,
            "Game Over",
            72f,
            FontStyles.Bold,
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 145f),
            new Vector2(760f, 120f));

        continueButton = CreateButton(
            "ContinueButton",
            gameOverPanel.transform,
            "Continue",
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, -35f));

        quitButton = CreateButton(
            "QuitButton",
            gameOverPanel.transform,
            "Quit",
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, -135f));

        gameOverPanel.SetActive(false);
    }

    private GameObject CreateUIObject(string objectName, Transform parent, params System.Type[] components)
    {
        System.Type[] allComponents = new System.Type[components.Length + 2];
        allComponents[0] = typeof(RectTransform);
        allComponents[1] = typeof(CanvasRenderer);

        for (int i = 0; i < components.Length; i++)
        {
            allComponents[i + 2] = components[i];
        }

        GameObject uiObject = new GameObject(objectName, allComponents);
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    private TMP_Text CreateText(
        string objectName,
        Transform parent,
        string text,
        float fontSize,
        FontStyles fontStyle,
        Vector2 anchor,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        GameObject textObject = CreateUIObject(objectName, parent, typeof(TextMeshProUGUI));
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = anchor;
        textRect.anchorMax = anchor;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = anchoredPosition;
        textRect.sizeDelta = sizeDelta;

        TMP_Text tmpText = textObject.GetComponent<TMP_Text>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.fontStyle = fontStyle;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;

        return tmpText;
    }

    private Button CreateButton(string objectName, Transform parent, string label, Vector2 anchor, Vector2 anchoredPosition)
    {
        GameObject buttonObject = CreateUIObject(objectName, parent, typeof(Image), typeof(Button));
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchor;
        buttonRect.anchorMax = anchor;
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = new Vector2(280f, 70f);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.18f, 0.22f, 0.28f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.18f, 0.22f, 0.28f, 1f);
        colors.highlightedColor = new Color(0.28f, 0.34f, 0.43f, 1f);
        colors.pressedColor = new Color(0.11f, 0.14f, 0.18f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        TMP_Text buttonText = CreateText(
            objectName + "Text",
            buttonObject.transform,
            label,
            30f,
            FontStyles.Bold,
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            buttonRect.sizeDelta);
        buttonText.raycastTarget = false;

        return button;
    }

    private void EnsureEventSystem()
    {
        EventSystem[] eventSystems = Resources.FindObjectsOfTypeAll<EventSystem>();
        for (int i = 0; i < eventSystems.Length; i++)
        {
            EventSystem eventSystem = eventSystems[i];
            if (eventSystem == null)
            {
                continue;
            }

            Scene scene = eventSystem.gameObject.scene;
            if (scene.IsValid() && scene.isLoaded)
            {
                return;
            }
        }

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        MoveToManagerScene(eventSystemObject);
    }

    private void BindButtonEvents()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    private void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void ActivateHierarchy(Transform target)
    {
        if (target == null)
        {
            return;
        }

        ActivateHierarchy(target.parent);

        if (!target.gameObject.activeSelf)
        {
            target.gameObject.SetActive(true);
        }
    }

    private void OnContinueClicked()
    {
        Time.timeScale = 1f;
        HideGameOverPanel();
        RespawnPlayer();
    }

    private void OnQuitClicked()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RespawnPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            Debug.LogWarning("Cannot respawn player because no object with the Player tag was found.");
            return;
        }

        player.SetActive(true);

        Vector3 targetPosition = usePlayerStartPosition && hasPlayerStartPosition
            ? playerStartPosition
            : respawnPosition;
        player.transform.position = targetPosition;

        if (StatsManager.Instance != null)
        {
            int clampedPercent = Mathf.Clamp(respawnHealthPercent, 1, 100);
            int respawnHealth = Mathf.Max(1, Mathf.RoundToInt(StatsManager.Instance.MaxHealth * (clampedPercent / 100f)));
            StatsManager.Instance.CurrentHealth = respawnHealth;
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.UpdateHealthUI();
        }
    }

    private void MoveToManagerScene(GameObject sceneObject)
    {
        Scene managerScene = gameObject.scene;
        if (managerScene.IsValid() && managerScene.isLoaded)
        {
            SceneManager.MoveGameObjectToScene(sceneObject, managerScene);
        }
    }
}
