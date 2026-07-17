using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换触发器，玩家触碰后切换场景
/// </summary>
public class SceneChange : MonoBehaviour
{
    [SerializeField] private float triggerReentryCooldown = 3f;

    private static float triggerBlockedUntil;

    [SerializeField] private string sceneToLoad; // 要加载的场景名称
    [SerializeField] private Animator fadeAnimator; // 淡入淡出动画控制器
    [SerializeField] private float fadeTime = 0.5f; // 淡入淡出时间
    [SerializeField] private Vector2 newPlayerPosition; // 玩家在新场景的位置
    [SerializeField] private CanvasGroup fadeCanvasGroup; // 淡入淡出Canvas组

    /// <summary>
    /// 游戏启动时关闭遮罩拦截
    /// </summary>
    private void Start()
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 玩家触碰触发器时切换场景
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.IsSceneTransitionBlocked || Time.unscaledTime < triggerBlockedUntil)
            {
                return;
            }

            // 检查是否有任务进行中
            if (QuestManager.Instance != null && QuestManager.Instance.HasActiveQuest())
            {
                Debug.Log("任务进行中，无法传送！请先完成或放弃任务。");
                return;
            }

            if (GameManager.instance != null)
            {
                BlockSceneTrigger();
                GameManager.instance.StartFade(sceneToLoad, newPlayerPosition, collision.transform);
            }
            else
            {
                BlockSceneTrigger();
                StartCoroutine(DelayFade(collision.transform));
            }
        }
    }

    private void BlockSceneTrigger()
    {
        triggerBlockedUntil = Time.unscaledTime + Mathf.Max(0.1f, triggerReentryCooldown);
    }

    /// <summary>
    /// 延迟淡入淡出协程
    /// </summary>
    private IEnumerator DelayFade(Transform targetPlayer)
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
        }

        if (fadeAnimator != null)
        {
            fadeAnimator.Play("FadeToWhite", 0, 0f);
        }

        yield return new WaitForSeconds(fadeTime);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = false;
        }

        SceneManager.LoadScene(sceneToLoad);

        if (targetPlayer != null)
        {
            targetPlayer.position = newPlayerPosition;
            Physics2D.SyncTransforms();
        }
    }
}
