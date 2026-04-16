using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private Vector2 newPlayerPosition;
    [SerializeField] private CanvasGroup fadeCanvasGroup; // 拖入 FadeCanvas
    private Transform player;

    private void Start()
    {
        // 游戏启动时关闭拦截，让 Fade_From_White 动画正常播放但不挡点击
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.blocksRaycasts = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 检查是否有任务进行中
            if (QuestManager.Instance != null && QuestManager.Instance.HasActiveQuest())
            {
                Debug.Log("任务进行中，无法传送！请先完成或放弃任务。");
                // 可以在这里显示提示UI
                return;
            }

            GameManager.instance.StartFade(sceneToLoad, newPlayerPosition, collision.transform);
        }
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        player.position = newPlayerPosition;
        fadeCanvasGroup.blocksRaycasts = false; // 切换场景前关闭拦截
        SceneManager.LoadScene(sceneToLoad);
    }
}