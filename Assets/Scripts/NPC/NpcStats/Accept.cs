using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // 显示选择按钮
    public void ShowChoices(DialogueSO dialogue)
    {
        Debug.Log("ShowChoices被调用");
        currentDialogue = dialogue;

        if (choicesPanel != null)
        {
            Debug.Log("显示Choices面板");
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

    // 隐藏选择按钮
    public void HideChoices()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
    }

    // Yes按钮 - 接受任务（金币奖励）
    private void OnYesClicked()
    {
        Debug.Log($"玩家接受了任务（金币奖励: {currentDialogue.goldReward}）");

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

    // No按钮 - 拒绝任务
    private void OnNoClicked()
    {
        Debug.Log("玩家拒绝了任务");

        // 直接关闭对话
        DialogueManager.Instance.EndDialogue();
        HideChoices();
    }

    // Option3按钮 - 稍后进行
    private void OnOption3Clicked()
    {
        Debug.Log("玩家选择稍后进行");

        // 关闭对话，不接受任务
        DialogueManager.Instance.EndDialogue();
        HideChoices();
    }

    // Option4按钮 - 接受任务（装备奖励）
    private void OnOption4Clicked()
    {
        Debug.Log($"玩家接受了任务（装备奖励: {currentDialogue.equipmentReward}）");

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
