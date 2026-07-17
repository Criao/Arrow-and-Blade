using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏管理器，管理场景切换和持久化对象
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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
    }

    /// <summary>
    /// 初始化单例，标记持久化对象
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            CleanupAndDestroy();
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPeristentObjects();
        }
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
    public void StartFade(string sceneToLoad, Vector2 newPlayerPosition, Transform player)
    {
        StartCoroutine(FadeRoutine(sceneToLoad, newPlayerPosition, player));
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

        if (player != null)
        {
            player.position = newPlayerPosition;
        }

        SceneManager.LoadScene(sceneToLoad);
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
                DontDestroyOnLoad(obj);
            }
        }
    }

    /// <summary>
    /// 清理并销毁重复的GameManager
    /// </summary>
    private void CleanupAndDestroy()
    {
        if (persistentObjects != null)
        {
            foreach (GameObject obj in persistentObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        Destroy(gameObject);
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
