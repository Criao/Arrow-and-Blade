using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏管理器，管理场景切换和持久化对象
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float sceneTransitionCooldown = 3f;

    private bool isSceneTransitioning;
    private bool hasPendingPlayerPosition;
    private Vector2 pendingPlayerPosition;
    private Transform pendingPlayer;
    private float sceneTransitionBlockedUntil;

    public static bool IsSceneTransitionBlocked =>
        instance != null &&
        (instance.isSceneTransitioning || Time.unscaledTime < instance.sceneTransitionBlockedUntil);
    [Header("Persitent Objects")]
    [SerializeField] private GameObject[] persistentObjects; // 跨场景持久化对象
    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // 淡入淡出Canvas组
    [SerializeField] private Animator fadeAnimator; // 淡入淡出动画控制器

    /// <summary>
    /// 游戏启动时关闭遮罩
    /// </summary>
    private void Start()
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.blocksRaycasts = false;

        ResolveCameraRig();
        ResolveAudioListeners();
        EventSystemResolver.EnsureSingleActiveEventSystem();
        BGMManager.PlayExplorationMusic();
    }

    /// <summary>
    /// 初始化单例，标记持久化对象
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        MarkPeristentObjects();
    }

    /// <summary>
    /// 启用时监听场景加载事件
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 禁用时取消监听
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// 场景加载完成时的回调
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyPendingPlayerPosition();
        RemoveDuplicatePlayers();
        ResolveCameraRig();
        ResolveAudioListeners();
        EventSystemResolver.EnsureSingleActiveEventSystem();
        BGMManager.PlayExplorationMusic();

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = false;
        }

        if (fadeAnimator != null)
        {
            fadeAnimator.Play("FadeFromWhite", 0, 0.5f);
        }
    }

    /// <summary>
    /// 开始淡入淡出场景切换
    /// </summary>
    private void ResolveCameraRig()
    {
        Transform playerTransform = GetActivePlayerTransform();
        Camera preferredCamera = FindPersistentComponent<Camera>();

        if (preferredCamera == null && Camera.main != null)
        {
            preferredCamera = Camera.main;
        }

        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (preferredCamera == null && cameras.Length > 0)
        {
            preferredCamera = cameras[0];
        }

        foreach (Camera camera in cameras)
        {
            if (camera == null)
            {
                continue;
            }

            bool isPreferredCamera = camera == preferredCamera;
            if (isPreferredCamera)
            {
                ActivateHierarchy(camera.transform);
            }

            camera.enabled = isPreferredCamera;

            CinemachineBrain brain = camera.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                brain.enabled = isPreferredCamera;
            }
        }

        ResolveVirtualCameras(playerTransform);
    }

    private void ResolveVirtualCameras(Transform playerTransform)
    {
        CinemachineVirtualCameraBase preferredVirtualCamera = FindPersistentComponent<CinemachineVirtualCameraBase>();
        CinemachineVirtualCameraBase[] virtualCameras = Object.FindObjectsByType<CinemachineVirtualCameraBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (preferredVirtualCamera == null && virtualCameras.Length > 0)
        {
            preferredVirtualCamera = virtualCameras[0];
        }

        foreach (CinemachineVirtualCameraBase virtualCamera in virtualCameras)
        {
            if (virtualCamera == null)
            {
                continue;
            }

            if (playerTransform != null)
            {
                virtualCamera.Follow = playerTransform;
            }

            bool isPreferredVirtualCamera = virtualCamera == preferredVirtualCamera;
            if (isPreferredVirtualCamera)
            {
                ActivateHierarchy(virtualCamera.transform);
                virtualCamera.enabled = true;
                virtualCamera.Priority = 20;
            }
            else
            {
                virtualCamera.Priority = 0;
                virtualCamera.enabled = false;
            }
        }
    }

    private Transform GetActivePlayerTransform()
    {
        if (PlayerHealth.ActiveInstance != null)
        {
            return PlayerHealth.ActiveInstance.transform;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null ? playerObject.transform : null;
    }

    private T FindPersistentComponent<T>() where T : Component
    {
        if (persistentObjects == null)
        {
            return null;
        }

        foreach (GameObject obj in persistentObjects)
        {
            if (obj == null)
            {
                continue;
            }

            T component = obj.GetComponentInChildren<T>(true);
            if (component != null)
            {
                return component;
            }
        }

        return null;
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

    private void ResolveAudioListeners()
    {
        AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (listeners.Length <= 0)
        {
            return;
        }

        AudioListener preferredListener = FindPersistentAudioListener(listeners);
        if (preferredListener == null && Camera.main != null)
        {
            preferredListener = Camera.main.GetComponent<AudioListener>();
        }

        if (preferredListener == null)
        {
            preferredListener = listeners[0];
        }

        foreach (AudioListener listener in listeners)
        {
            if (listener != null)
            {
                listener.enabled = listener == preferredListener;
            }
        }
    }

    private AudioListener FindPersistentAudioListener(AudioListener[] listeners)
    {
        if (persistentObjects == null)
        {
            return null;
        }

        foreach (GameObject obj in persistentObjects)
        {
            if (obj == null)
            {
                continue;
            }

            foreach (AudioListener listener in listeners)
            {
                if (listener != null && listener.transform.IsChildOf(obj.transform))
                {
                    return listener;
                }
            }
        }

        return null;
    }

    public bool StartFade(string sceneToLoad, Vector2 newPlayerPosition, Transform player)
    {
        if (isSceneTransitioning || Time.unscaledTime < sceneTransitionBlockedUntil)
        {
            return false;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Cannot change scene because sceneToLoad is missing.");
            return false;
        }

        isSceneTransitioning = true;
        sceneTransitionBlockedUntil = Time.unscaledTime + Mathf.Max(0.1f, sceneTransitionCooldown);
        StartCoroutine(FadeRoutine(sceneToLoad, newPlayerPosition, player));
        return true;
    }

    /// <summary>
    /// 淡入淡出协程
    /// </summary>
    private IEnumerator FadeRoutine(string sceneToLoad, Vector2 newPlayerPosition, Transform player)
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
        }

        if (fadeAnimator != null)
        {
            fadeAnimator.Play("FadeToWhite", 0, 0f);
        }

        yield return new WaitForSeconds(0.5f);

        pendingPlayer = player;
        pendingPlayerPosition = newPlayerPosition;
        hasPendingPlayerPosition = true;

        SceneManager.LoadScene(sceneToLoad);
    }

    private void ApplyPendingPlayerPosition()
    {
        if (!hasPendingPlayerPosition)
        {
            return;
        }

        Transform targetPlayer = pendingPlayer;
        if (targetPlayer == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            targetPlayer = playerObject != null ? playerObject.transform : null;
        }

        if (targetPlayer != null)
        {
            if (!targetPlayer.gameObject.activeSelf)
            {
                targetPlayer.gameObject.SetActive(true);
            }

            targetPlayer.position = pendingPlayerPosition;

            Rigidbody2D playerBody = targetPlayer.GetComponent<Rigidbody2D>();
            if (playerBody != null)
            {
                playerBody.velocity = Vector2.zero;
            }

            PlayerMoveMent movement = targetPlayer.GetComponent<PlayerMoveMent>();
            if (movement != null && movement.CurrentState != PlayerState.Dead)
            {
                movement.ClearTemporaryStates();
            }

            Physics2D.SyncTransforms();
        }

        pendingPlayer = null;
        hasPendingPlayerPosition = false;
        isSceneTransitioning = false;
        sceneTransitionBlockedUntil = Time.unscaledTime + Mathf.Max(0.1f, sceneTransitionCooldown);
    }

    private void RemoveDuplicatePlayers()
    {
        PlayerHealth activePlayer = PlayerHealth.ActiveInstance;
        if (activePlayer == null)
        {
            return;
        }

        PlayerHealth[] players = Object.FindObjectsByType<PlayerHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PlayerHealth player in players)
        {
            if (player == null || player == activePlayer)
            {
                continue;
            }

            player.gameObject.SetActive(false);
            Destroy(player.gameObject);
        }
    }

    /// <summary>
    /// 标记持久化对象，使其跨场景保留
    /// </summary>
    private void MarkPeristentObjects()
    {
        if (persistentObjects == null)
        {
            return;
        }

        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                if (obj.GetComponent<EventSystem>() != null)
                {
                    continue;
                }

                DontDestroyOnLoad(obj);
            }
        }
    }

}

public static class PauseManager
{
    private static readonly HashSet<string> PauseOwners = new HashSet<string>();

    public static bool IsPaused => PauseOwners.Count > 0;

    public static void SetPaused(string owner, bool paused)
    {
        if (string.IsNullOrEmpty(owner))
        {
            return;
        }

        if (paused)
        {
            PauseOwners.Add(owner);
        }
        else
        {
            PauseOwners.Remove(owner);
        }

        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public static void ClearAll()
    {
        PauseOwners.Clear();
        Time.timeScale = 1f;
    }
}

public static class EventSystemResolver
{
    public static bool EnsureSingleActiveEventSystem()
    {
        EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        EventSystem preferred = GetPreferredEventSystem(eventSystems);
        if (preferred == null)
        {
            return false;
        }

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem == null || eventSystem == preferred)
            {
                continue;
            }

            eventSystem.enabled = false;

            if (eventSystem.gameObject.activeSelf)
            {
                eventSystem.gameObject.SetActive(false);
            }
        }

        if (preferred.gameObject.activeInHierarchy && !preferred.enabled)
        {
            preferred.enabled = true;
        }

        return true;
    }

    private static EventSystem GetPreferredEventSystem(EventSystem[] eventSystems)
    {
        EventSystem current = EventSystem.current;
        if (IsLoadedEventSystem(current) && current.isActiveAndEnabled)
        {
            return current;
        }

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (IsLoadedEventSystem(eventSystem) && eventSystem.isActiveAndEnabled)
            {
                return eventSystem;
            }
        }

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (IsLoadedEventSystem(eventSystem) && eventSystem.gameObject.activeInHierarchy)
            {
                return eventSystem;
            }
        }

        return null;
    }

    private static bool IsLoadedEventSystem(EventSystem eventSystem)
    {
        if (eventSystem == null)
        {
            return false;
        }

        Scene scene = eventSystem.gameObject.scene;
        return scene.IsValid() && scene.isLoaded;
    }
}

public static class BGMManager
{
    private const string ExplorationMusicPath = "Audio/BGM/peaceful_field_light_drums_loop_45s";
    private const float DefaultVolume = 0.32f;
    private const float DefaultFadeDuration = 1f;

    private static AudioSource audioSource;
    private static BGMCoroutineRunner coroutineRunner;
    private static Coroutine fadeRoutine;
    private static string currentClipPath;

    public static void PlayExplorationMusic()
    {
        PlayMusic(ExplorationMusicPath, DefaultVolume, DefaultFadeDuration);
    }

    public static void PlayMusic(string resourcePath, float volume = DefaultVolume, float fadeDuration = DefaultFadeDuration)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            return;
        }

        EnsureAudioSource();
        if (audioSource == null || coroutineRunner == null)
        {
            return;
        }

        if (currentClipPath == resourcePath && audioSource.isPlaying)
        {
            audioSource.volume = volume;
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip == null)
        {
            Debug.LogWarning("Missing BGM clip: " + resourcePath);
            return;
        }

        if (fadeRoutine != null)
        {
            coroutineRunner.StopCoroutine(fadeRoutine);
        }

        fadeRoutine = coroutineRunner.StartCoroutine(FadeToClip(clip, resourcePath, Mathf.Clamp01(volume), Mathf.Max(0f, fadeDuration)));
    }

    public static void StopMusic(float fadeDuration = DefaultFadeDuration)
    {
        EnsureAudioSource();
        if (audioSource == null || coroutineRunner == null)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            coroutineRunner.StopCoroutine(fadeRoutine);
        }

        fadeRoutine = coroutineRunner.StartCoroutine(FadeOut(Mathf.Max(0f, fadeDuration)));
    }

    private static void EnsureAudioSource()
    {
        if (audioSource != null)
        {
            if (coroutineRunner == null)
            {
                coroutineRunner = audioSource.GetComponent<BGMCoroutineRunner>();
                if (coroutineRunner == null)
                {
                    coroutineRunner = audioSource.gameObject.AddComponent<BGMCoroutineRunner>();
                }
            }
            return;
        }

        GameObject musicObject = new GameObject("BGM Manager");
        Object.DontDestroyOnLoad(musicObject);

        coroutineRunner = musicObject.AddComponent<BGMCoroutineRunner>();
        audioSource = musicObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 0f;
    }

    private static IEnumerator FadeToClip(AudioClip clip, string clipPath, float targetVolume, float fadeDuration)
    {
        if (audioSource.isPlaying && fadeDuration > 0f)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = fadeDuration > 0f ? 0f : targetVolume;
        audioSource.Play();
        currentClipPath = clipPath;

        if (fadeDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeDuration);
                yield return null;
            }
        }

        audioSource.volume = targetVolume;
        fadeRoutine = null;
    }

    private static IEnumerator FadeOut(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        if (fadeDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = 0f;
        currentClipPath = null;
        fadeRoutine = null;
    }
}

public sealed class BGMCoroutineRunner : MonoBehaviour
{
}
