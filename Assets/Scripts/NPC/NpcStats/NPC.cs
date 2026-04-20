using System.Collections;
using UnityEngine;

/// <summary>
/// NPC控制器，管理NPC的状态切换（巡逻/对话）
/// </summary>
public class NPC : MonoBehaviour
{
    /// <summary>
    /// NPC状态枚举
    /// </summary>
    public enum NPCState { Patrol, Talk }
    public NPCState currentState = NPCState.Patrol;

    [SerializeField] private NPC_Patrol npcPatrol; // 巡逻组件
    [SerializeField] private NPC_Talk npcTalk; // 对话组件

    [Header("Quest Settings")]
    public string questID; // 该NPC关联的任务ID

    /// <summary>
    /// 初始化，设置为巡逻状态并监听任务完成事件
    /// </summary>
    private void Start()
    {
        SetState(NPCState.Patrol);

        // 监听任务完成事件
        QuestManager.OnQuestCompleted += OnQuestCompleted;
    }

    /// <summary>
    /// 销毁时取消事件监听
    /// </summary>
    private void OnDestroy()
    {
        QuestManager.OnQuestCompleted -= OnQuestCompleted;
    }

    /// <summary>
    /// 任务完成时的回调
    /// </summary>
    private void OnQuestCompleted(QuestData quest)
    {
        // 如果完成的是该NPC的任务，重新显示NPC
        if (quest.questID == questID)
        {
            gameObject.SetActive(true);
            Debug.Log($"任务完成，NPC {gameObject.name} 重新出现");
        }
    }

    /// <summary>
    /// 设置NPC状态
    /// </summary>
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

    /// <summary>
    /// 启动关闭动画（由NPC物体执行协程，避免NPC_Talk禁用后协程中断）
    /// </summary>
    public void StartCloseAnimation(Animator animator, GameObject obj)
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartCloseAnimation(animator, obj);
        }
        else
        {
            Debug.LogWarning("DialogueManager.Instance 为空，无法执行关闭动画");
            if (obj != null)
                obj.SetActive(false);
        }
    }

    /// <summary>
    /// 播放关闭动画并隐藏对象
    /// </summary>
    private IEnumerator CloseAndHide(Animator animator, GameObject obj)
    {
        animator.Play("Close");
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
    }
}