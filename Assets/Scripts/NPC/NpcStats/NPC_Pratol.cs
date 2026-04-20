using System.Collections;
using UnityEngine;

/// <summary>
/// NPC巡逻行为，控制NPC在多个巡逻点之间移动
/// </summary>
public class NPC_Patrol : MonoBehaviour
{
    [SerializeField] private Vector2[] patrolPoints; // 巡逻点数组
    [SerializeField] private float speed = 2f; // 移动速度
    [SerializeField] private float reachDistance = 0.2f; // 到达巡逻点的判定距离
    [SerializeField] private float waitTime = 3f; // 在巡逻点等待的时间
    [SerializeField] private Animator animator; // 动画控制器
    private Rigidbody2D rb;
    private int currentIndex = 0; // 当前巡逻点索引
    private bool isWaiting = false; // 是否正在等待
    private bool hasStarted = false; // 是否已开始巡逻
    private bool isStopped = false; // 是否被玩家触发停止
    private Coroutine waitCoroutine; // 等待协程引用，防止重复启动

    /// <summary>
    /// 初始化，延迟开始巡逻
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator.Play("Idle");
        StartCoroutine(DelayStart());
    }

    /// <summary>
    /// 延迟开始巡逻
    /// </summary>
    private IEnumerator DelayStart()
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);
        hasStarted = true;
        animator.Play("Walk");
    }

    /// <summary>
    /// 每帧更新，控制NPC向目标巡逻点移动
    /// </summary>
    private void Update()
    {
        if (patrolPoints.Length == 0 || isWaiting || !hasStarted || isStopped) return;

        Vector2 target = patrolPoints[currentIndex];
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;

        // 根据移动方向翻转角色
        if (direction.x > 0)
            animator.transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            animator.transform.localScale = new Vector3(-1, 1, 1);

        // 到达巡逻点后开始等待
        if (Vector2.Distance(transform.position, target) < reachDistance && waitCoroutine == null)
        {
            waitCoroutine = StartCoroutine(WaitAtPoint());
        }
    }

    /// <summary>
    /// 停止巡逻（玩家触发对话时调用）
    /// </summary>
    public void StopPatrol()
    {
        isStopped = true;
        rb.velocity = Vector2.zero;
        animator.Play("Idle");
    }

    /// <summary>
    /// 恢复巡逻（玩家离开对话范围时调用）
    /// </summary>
    public void ResumePatrol()
    {
        isStopped = false;
        if (hasStarted && !isWaiting && gameObject.activeInHierarchy)
            animator.Play("Walk");
    }

    /// <summary>
    /// 在巡逻点等待
    /// </summary>
    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        rb.velocity = Vector2.zero;
        animator.Play("Idle");
        yield return new WaitForSeconds(waitTime);
        currentIndex = (currentIndex + 1) % patrolPoints.Length;
        isWaiting = false;
        waitCoroutine = null;
        if (!isStopped)
            animator.Play("Walk");
    }
}