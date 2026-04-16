// NPC.cs
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NPCState { Patrol, Talk }
    public NPCState currentState = NPCState.Patrol;

    [SerializeField] private NPC_Patrol npcPatrol;
    [SerializeField] private NPC_Talk npcTalk;

    [Header("Quest Settings")]
    public string questID; // 该NPC关联的任务ID

    private void Start()
    {
        SetState(NPCState.Patrol);

        // 监听任务完成事件
        QuestManager.OnQuestCompleted += OnQuestCompleted;
    }

    private void OnDestroy()
    {
        // 取消监听
        QuestManager.OnQuestCompleted -= OnQuestCompleted;
    }

    // 任务完成时的回调
    private void OnQuestCompleted(QuestData quest)
    {
        // 如果完成的是该NPC的任务，重新显示NPC
        if (quest.questID == questID)
        {
            gameObject.SetActive(true);
            Debug.Log($"任务完成，NPC {gameObject.name} 重新出现");
        }
    }

    public void SetState(NPCState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case NPCState.Patrol:
                npcPatrol.enabled = true;
                npcTalk.enabled = false;
                npcPatrol.ResumePatrol();
                break;
            case NPCState.Talk:
                npcPatrol.enabled = false;
                npcTalk.enabled = true;
                npcPatrol.StopPatrol();
                break;
        }
    }

    // ← 新增，由 NPC 物体执行协程，避免 NPC_Talk 禁用后协程中断
    public void StartCloseAnimation(Animator animator, GameObject obj)
    {
        // 检查 DialogueManager 是否存在
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartCloseAnimation(animator, obj);
        }
        else
        {
            Debug.LogWarning("DialogueManager.Instance 为空，无法执行关闭动画");
            // 直接隐藏对象
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private IEnumerator CloseAndHide(Animator animator, GameObject obj)
    {
        animator.Play("Close");
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
    }
}