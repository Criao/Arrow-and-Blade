using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Persitent Objects")]
    [SerializeField] private GameObject[] persistentObjects;
    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // 拖入 FadeCanvas 的 CanvasGroup
    [SerializeField] private Animator fadeAnimator;

    private void Start()
    {
        // 游戏启动时也要关闭遮罩
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.blocksRaycasts = false;
    }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeCanvasGroup.blocksRaycasts = false;
        fadeAnimator.Play("FadeFromWhite", 0, 0.5f); // 强制从头播放淡入动画
    }
    public void StartFade(string sceneToLoad, Vector2 newPlayerPosition, Transform player)
    {
        StartCoroutine(FadeRoutine(sceneToLoad, newPlayerPosition, player));
    }

    private IEnumerator FadeRoutine(string sceneToLoad, Vector2 newPlayerPosition, Transform player)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        fadeAnimator.Play("FadeToWhite", 0, 0f); // 强制从头播放淡出动画
        yield return new WaitForSeconds(0.5f);
        player.position = newPlayerPosition;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void MarkPeristentObjects()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanupAndDestroy()
    {
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }
}