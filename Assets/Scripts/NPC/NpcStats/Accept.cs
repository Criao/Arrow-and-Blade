using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 任务接受UI，处理玩家接受/拒绝任务的选择
/// </summary>
public class Accept : MonoBehaviour
{
    [Header("Button References")]
    public Button yesButton; // 接受任务（金币奖励）
    public Button noButton; // 拒绝任务
    public Button lateButton; // 稍后进行
    public Button acceptButton; // 接受任务（装备奖励）

    [Header("Button Text References")]
    public TMP_Text yesButtonText; // Yes按钮文本
    public TMP_Text noButtonText; // No按钮文本
    public TMP_Text option3ButtonText; // Option3按钮文本
    public TMP_Text option4ButtonText; // Option4按钮文本

    [Header("Panel Reference")]
    public GameObject choicesPanel; // Choices面板

    [Header("Quest References")]
    public NPC npcController; // NPC控制器
    public EnemySpawner enemySpawner; // 敌人生成器

    private DialogueSO currentDialogue; // 当前对话数据

    /// <summary>
    /// 初始化，绑定按钮事件
    /// </summary>
    private void Start()
    {
        // 绑定按钮事件
        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);

        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);

        if (lateButton != null)
            lateButton.onClick.AddListener(OnOption3Clicked);

        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnOption4Clicked);

        // 初始隐藏选择面板
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
    }

    /// <summary>
    /// 显示任务选择按钮
    /// </summary>
    public void ShowChoices(DialogueSO dialogue)
    {
        currentDialogue = dialogue;

        if (choicesPanel != null)
        {
            choicesPanel.SetActive(true);

            // 设置按钮文本（英文）
            if (yesButtonText != null)
                yesButtonText.text = $"Accept\n(Gold {dialogue.goldReward})";

            if (option3ButtonText != null)
                option3ButtonText.text = "Later";

            if (option4ButtonText != null)
                option4ButtonText.text = $"Accept\n({dialogue.equipmentReward})";

            if (noButtonText != null)
                noButtonText.text = "Decline";
        }
        else
        {
            Debug.LogError("Choices面板引用为空！请在Accept脚本中拖入Choices面板");
        }
    }

    /// <summary>
    /// 隐藏任务选择按钮
    /// </summary>
    public void HideChoices()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
    }

    /// <summary>
    /// Yes按钮点击 - 接受任务（金币奖励）
    /// </summary>
    private void OnYesClicked()
    {

        // 接受任务
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AcceptQuest(
                currentDialogue.questID,
                "Kill Goblins",
                currentDialogue.questDescription,
                7, // Kill 7 goblins
                RewardType.Gold,
                currentDialogue.goldReward,
                currentDialogue.equipmentReward,
                "Enemy" // 哥布林的Tag
            );

            // 隐藏NPC
            if (npcController != null)
            {
                npcController.gameObject.SetActive(false);
            }

            // 生成敌人
            if (enemySpawner != null)
            {
                enemySpawner.SpawnEnemies();
            }
        }

        // 关闭对话
        DialogueManager.Instance.EndDialogue();
        HideChoices();
    }

    /// <summary>
    /// No按钮点击 - 拒绝任务
    /// </summary>
    private void OnNoClicked()
    {

        // 直接关闭对话
        DialogueManager.Instance.EndDialogue();
        HideChoices();
    }

    /// <summary>
    /// Option3按钮点击 - 稍后进行
    /// </summary>
    private void OnOption3Clicked()
    {

        // 关闭对话，不接受任务
        DialogueManager.Instance.EndDialogue();
        HideChoices();
    }

    /// <summary>
    /// Option4按钮点击 - 接受任务（装备奖励）
    /// </summary>
    private void OnOption4Clicked()
    {

        // 接受任务
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AcceptQuest(
                currentDialogue.questID,
                "Kill Goblins",
                currentDialogue.questDescription,
                7, // Kill 7 goblins
                RewardType.Equipment,
                currentDialogue.goldReward,
                currentDialogue.equipmentReward,
                "Enemy" // 哥布林的Tag
            );

            // 隐藏NPC
            if (npcController != null)
            {
                npcController.gameObject.SetActive(false);
            }

            // 生成敌人
            if (enemySpawner != null)
            {
                enemySpawner.SpawnEnemies();
            }
        }

        // 关闭对话
        DialogueManager.Instance.EndDialogue();
        HideChoices();
    }
}
