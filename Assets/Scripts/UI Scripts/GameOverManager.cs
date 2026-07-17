using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
    private const string PauseOwner = nameof(GameOverManager);
    private const string ButtonSpritePath = "UI/Button_Blue_9Slides";
    private const string BangersFontPath = "Fonts & Materials/Bangers SDF";
    private const string BangersDropShadowMaterialPath = "Fonts & Materials/Bangers SDF - Drop Shadow";

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
    private PanelMode currentMode = PanelMode.Hidden;
    private static Sprite cachedButtonSprite;
    private static TMP_FontAsset cachedBangersFont;
    private static Material cachedBangersDropShadowMaterial;

    private enum PanelMode
    {
        Hidden,
        Pause,
        GameOver
    }

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
            gameObject.SetActive(false);
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

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (currentMode == PanelMode.GameOver)
        {
            return;
        }

        if (currentMode == PanelMode.Pause)
        {
            HidePausePanel();
            return;
        }

        if (PauseManager.IsPaused)
        {
            return;
        }

        ShowPause();
    }

    private void OnDestroy()
    {
        PauseManager.SetPaused(PauseOwner, false);

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

        currentMode = PanelMode.GameOver;
        SetPanelText("Game Over", "Continue", "Quit");
        ActivateHierarchy(gameOverPanel.transform);
        EnsureFullscreenDimmer();
        gameOverPanel.SetActive(true);
        PauseManager.SetPaused(PauseOwner, true);
    }

    public void ShowPause()
    {
        EnsureReady();

        if (gameOverPanel == null)
        {
            Debug.LogError("Pause panel is missing and could not be created.");
            return;
        }

        currentMode = PanelMode.Pause;
        SetPanelText("Pause", "Continue", "Exit");
        ActivateHierarchy(gameOverPanel.transform);
        EnsureFullscreenDimmer();
        gameOverPanel.SetActive(true);
        PauseManager.SetPaused(PauseOwner, true);
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
        panelImage.color = new Color(0f, 0f, 0f, 0f);

        GameObject titleBackdrop = CreateUIObject("TitleBackdrop", gameOverPanel.transform, typeof(Image));
        RectTransform titleBackdropRect = titleBackdrop.GetComponent<RectTransform>();
        titleBackdropRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleBackdropRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleBackdropRect.pivot = new Vector2(0.5f, 0.5f);
        titleBackdropRect.anchoredPosition = new Vector2(0f, 245f);
        titleBackdropRect.sizeDelta = new Vector2(760f, 150f);

        Image titleBackdropImage = titleBackdrop.GetComponent<Image>();
        titleBackdropImage.color = new Color(0f, 0f, 0f, 0.5f);

        gameOverText = CreateText(
            "GameOverText",
            gameOverPanel.transform,
            "Game Over",
            108f,
            FontStyles.Normal,
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 245f),
            new Vector2(820f, 160f));
        ApplyBangersFont(gameOverText, true);

        continueButton = CreateButton(
            "ContinueButton",
            gameOverPanel.transform,
            "Continue",
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, -30f));

        quitButton = CreateButton(
            "QuitButton",
            gameOverPanel.transform,
            "Quit",
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, -235f));

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
        buttonRect.sizeDelta = new Vector2(450f, 135f);

        Image buttonImage = buttonObject.GetComponent<Image>();
        Sprite buttonSprite = GetButtonSprite();
        if (buttonSprite != null)
        {
            buttonImage.sprite = buttonSprite;
            buttonImage.type = Image.Type.Sliced;
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = new Color(0.72f, 0.84f, 0.88f, 1f);

            Outline buttonOutline = buttonObject.AddComponent<Outline>();
            buttonOutline.effectColor = new Color(0.12f, 0.14f, 0.18f, 0.95f);
            buttonOutline.effectDistance = new Vector2(4f, -4f);
        }

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
        colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
        colors.disabledColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        TMP_Text buttonText = CreateText(
            objectName + "Text",
            buttonObject.transform,
            label,
            44f,
            FontStyles.Normal,
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            buttonRect.sizeDelta);
        buttonText.color = new Color(0.196f, 0.196f, 0.196f, 1f);
        ApplyBangersFont(buttonText, false);
        buttonText.raycastTarget = false;

        return button;
    }

    private static Sprite GetButtonSprite()
    {
        if (cachedButtonSprite == null)
        {
            cachedButtonSprite = Resources.Load<Sprite>(ButtonSpritePath);
        }

        return cachedButtonSprite;
    }

    private static TMP_FontAsset GetBangersFont()
    {
        if (cachedBangersFont == null)
        {
            cachedBangersFont = Resources.Load<TMP_FontAsset>(BangersFontPath);
        }

        return cachedBangersFont;
    }

    private static Material GetBangersDropShadowMaterial()
    {
        if (cachedBangersDropShadowMaterial == null)
        {
            cachedBangersDropShadowMaterial = Resources.Load<Material>(BangersDropShadowMaterialPath);
        }

        return cachedBangersDropShadowMaterial;
    }

    private static void ApplyBangersFont(TMP_Text text, bool useDropShadow)
    {
        if (text == null)
        {
            return;
        }

        TMP_FontAsset font = GetBangersFont();
        if (font != null)
        {
            text.font = font;
        }

        if (useDropShadow)
        {
            Material dropShadowMaterial = GetBangersDropShadowMaterial();
            if (dropShadowMaterial != null)
            {
                text.fontSharedMaterial = dropShadowMaterial;
            }
        }
    }

    private void EnsureEventSystem()
    {
        if (EventSystemResolver.EnsureSingleActiveEventSystem())
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        MoveToManagerScene(eventSystemObject);
        EventSystemResolver.EnsureSingleActiveEventSystem();
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

        SetFullscreenDimmerVisible(false);
        currentMode = PanelMode.Hidden;
    }

    private void HidePausePanel()
    {
        if (currentMode != PanelMode.Pause)
        {
            return;
        }

        PauseManager.SetPaused(PauseOwner, false);
        HideGameOverPanel();
    }

    private void SetPanelText(string title, string continueLabel, string quitLabel)
    {
        if (gameOverText != null)
        {
            gameOverText.text = title;
        }

        SetButtonLabel(continueButton, continueLabel);
        SetButtonLabel(quitButton, quitLabel);
    }

    private void SetButtonLabel(Button button, string label)
    {
        if (button == null)
        {
            return;
        }

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>(true);
        if (buttonText != null)
        {
            buttonText.text = label;
        }
    }

    private void EnsureFullscreenDimmer()
    {
        if (gameOverPanel == null)
        {
            return;
        }

        Transform dimmerParent = gameOverPanel.transform.parent != null
            ? gameOverPanel.transform.parent
            : gameOverPanel.transform;

        Transform existingDimmer = dimmerParent.Find("FullscreenDimmer");
        if (existingDimmer != null)
        {
            Image existingImage = existingDimmer.GetComponent<Image>();
            if (existingImage != null)
            {
                existingImage.color = new Color(0f, 0f, 0f, 0.58f);
            }

            existingDimmer.gameObject.SetActive(true);
            existingDimmer.SetAsFirstSibling();
            return;
        }

        GameObject dimmer = CreateUIObject("FullscreenDimmer", dimmerParent, typeof(Image));
        RectTransform dimmerRect = dimmer.GetComponent<RectTransform>();
        dimmerRect.anchorMin = Vector2.zero;
        dimmerRect.anchorMax = Vector2.one;
        dimmerRect.offsetMin = Vector2.zero;
        dimmerRect.offsetMax = Vector2.zero;

        Image dimmerImage = dimmer.GetComponent<Image>();
        dimmerImage.color = new Color(0f, 0f, 0f, 0.58f);
        dimmerImage.raycastTarget = false;
        dimmer.transform.SetAsFirstSibling();
    }

    private void SetFullscreenDimmerVisible(bool visible)
    {
        if (gameOverPanel == null)
        {
            return;
        }

        Transform dimmerParent = gameOverPanel.transform.parent != null
            ? gameOverPanel.transform.parent
            : gameOverPanel.transform;

        Transform existingDimmer = dimmerParent.Find("FullscreenDimmer");
        if (existingDimmer != null)
        {
            existingDimmer.gameObject.SetActive(visible);
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
        PauseManager.SetPaused(PauseOwner, false);

        if (currentMode == PanelMode.Pause)
        {
            HideGameOverPanel();
            return;
        }

        HideGameOverPanel();
        RespawnPlayer();
    }

    private void OnQuitClicked()
    {
        PauseManager.SetPaused(PauseOwner, false);

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

        PlayerMoveMent movement = player.GetComponent<PlayerMoveMent>();
        if (movement != null)
        {
            movement.Revive();
        }

        if (StatsManager.Instance != null)
        {
            int clampedPercent = Mathf.Clamp(respawnHealthPercent, 1, 100);
            int respawnHealth = Mathf.Max(1, Mathf.RoundToInt(StatsManager.Instance.MaxHealth * (clampedPercent / 100f)));
            StatsManager.Instance.CurrentHealth = respawnHealth;
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ResetDeathState();
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
