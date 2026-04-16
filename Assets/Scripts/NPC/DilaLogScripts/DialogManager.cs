
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [Header("UI References")]
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel; // ← 拖入对话面板物体
    public bool isDilaogueActive;

    [Header("Quest System")]
    public Accept acceptScript; // Accept脚本引用

    private DialogueSO currentDialogue;
    private int dialogueIndex;

    private void Awake()
    {
        Debug.Log("DialogueManager Awake 被调用");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保留DialogCanvas
            Debug.Log("DialogueManager Instance 已设置");
        }
        else
        {
            Debug.LogWarning("场景中已存在DialogueManager，销毁重复的对象");
            Destroy(gameObject);
            return; // 销毁后直接返回，不执行后续代码
        }

        // 确保 DialogCanvas 始终激活
        gameObject.SetActive(true);

        // 永远不禁用 DialogCanvas，只控制 CanvasGroup 透明度
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
    public void StartDialogue(DialogueSO dialogueSO)
    {
        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDilaogueActive = true;
        dialoguePanel.SetActive(true);
        // ← 确保 Canvas Group 可交互
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (dialogueIndex < currentDialogue.lines.Length)
        {
            ShowDialogue();
        }
        else
        {
            // 对话结束，检查是否有任务
            if (currentDialogue.hasQuest)
            {
                ShowQuestChoices(); // 显示任务选择按钮
            }
            else
            {
                EndDialogue(); // 直接结束对话
            }
        }
    }

    private void ShowDialogue()
    {
        DialogueLine line = currentDialogue.lines[dialogueIndex];
        portrait.sprite = line.speaker.portrait;
        actorName.text = line.speaker.actorName;
        dialogueText.text = line.text;
        dialogueIndex++;
    }

    public void EndDialogue()
    {
        isDilaogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // 隐藏选择按钮
        if (acceptScript != null)
            acceptScript.HideChoices();

        // 隐藏对话面板，但保持Canvas Group透明度控制
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        currentDialogue = null;
        dialogueIndex = 0;
    }

    // 显示任务选择按钮
    private void ShowQuestChoices()
    {
        Debug.Log("尝试显示任务选择按钮");
        if (acceptScript != null)
        {
            Debug.Log("Accept脚本存在，调用ShowChoices");
            acceptScript.ShowChoices(currentDialogue); // 传递当前对话数据
        }
        else
        {
            Debug.LogError("Accept脚本引用为空！请在DialogueManager中拖入Accept脚本");
        }
    }

    public void StartCloseAnimation(Animator anim, GameObject obj)
    {
        if (this != null && gameObject != null)
        {
            // 确保DialogCanvas是激活的，才能启动协程
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            StartCoroutine(CloseAndHide(anim, obj));
        }
    }

    private IEnumerator CloseAndHide(Animator anim, GameObject obj)
    {
        // 检查对象是否存在且激活
        if (anim != null && anim.gameObject.activeInHierarchy)
        {
            anim.Play("Close");
            yield return new WaitForSeconds(0.5f);
        }

        // 隐藏对象
        if (obj != null)
            obj.SetActive(false);
    }
}