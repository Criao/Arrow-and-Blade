using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏结束管理器，处理玩家死亡后的 Game Over 界面
/// </summary>
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // Game Over 面板
    public TMP_Text gameOverText; // "Game Over" 文本
    public Button continueButton; // 继续游戏按钮
    public Button quitButton; // 退出游戏按钮

    [Header("Continue Settings")]
    public bool usePlayerStartPosition = true; // 是否使用玩家起始位置作为重生点
    public Vector3 respawnPosition; // 重生位置（如果不使用起始位置）
    public int respawnHealthPercent = 50; // 重生时的生命值百分比

    private GameObject player; // 玩家对象引用
    private Vector3 playerStartPosition; // 玩家起始位置

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 查找玩家对象
        player = GameObject.FindGameObjectWithTag("Player");

        // 记录玩家起始位置
        if (player != null && usePlayerStartPosition)
        {
            playerStartPosition = player.transform.position;
            Debug.Log($"记录玩家起始位置: {playerStartPosition}");
        }

        // 绑定按钮事件
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        // 初始隐藏 Game Over 面板
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// 显示 Game Over 界面
    /// </summary>
    public void ShowGameOver()
    {
        Debug.Log("ShowGameOver 被调用");

        if (gameOverPanel != null)
        {
            Debug.Log("gameOverPanel 不为空，正在激活");
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // 暂停游戏
            Debug.Log($"Game Over 面板已激活，当前状态: {gameOverPanel.activeSelf}");
        }
        else
        {
            Debug.LogError("gameOverPanel 为空！请在 Inspector 中拖入 GameOver Panel");
        }
    }

    /// <summary>
    /// 继续游戏按钮点击事件
    /// </summary>
    private void OnContinueClicked()
    {
        // 恢复游戏时间
        Time.timeScale = 1f;

        // 隐藏 Game Over 面板
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 重生玩家
        RespawnPlayer();
    }

    /// <summary>
    /// 退出游戏按钮点击事件
    /// </summary>
    private void OnQuitClicked()
    {
        // 恢复游戏时间
        Time.timeScale = 1f;

        // 退出游戏（在编辑器中会停止播放，在构建版本中会退出应用）
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// 重生玩家
    /// </summary>
    private void RespawnPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            // 重新激活玩家
            player.SetActive(true);

            // 设置重生位置（使用起始位置或指定位置）
            Vector3 targetPosition = usePlayerStartPosition ? playerStartPosition : respawnPosition;
            player.transform.position = targetPosition;
            Debug.Log($"玩家重生在位置: {targetPosition}");

            // 恢复生命值
            int respawnHealth = Mathf.RoundToInt(StatsManager.Instance.MaxHealth * (respawnHealthPercent / 100f));
            StatsManager.Instance.CurrentHealth = respawnHealth;

            // 更新 UI
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.UpdateHealthUI();
            }

            Debug.Log($"玩家重生，生命值: {StatsManager.Instance.CurrentHealth}/{StatsManager.Instance.MaxHealth}");
        }
    }

    /// <summary>
    /// 设置重生位置（可以从外部调用，比如在存档点）
    /// </summary>
    public void SetRespawnPosition(Vector3 position)
    {
        respawnPosition = position;
    }
}
