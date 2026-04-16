using System.Collections;
using UnityEngine;

public class NPC_Patrol : MonoBehaviour
{
    [SerializeField] private Vector2[] patrolPoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reachDistance = 0.2f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private int currentIndex = 0;
    private bool isWaiting = false;
    private bool hasStarted = false;
    private bool isStopped = false; // ← 新增，玩家触发停止
    private Coroutine waitCoroutine; // ← 记录协程，防止重复启动

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator.Play("Idle");
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);
        hasStarted = true;
        animator.Play("Walk");
    }

    private void Update()
    {
        if (patrolPoints.Length == 0 || isWaiting || !hasStarted || isStopped) return;

        Vector2 target = patrolPoints[currentIndex];
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;

        if (direction.x > 0)
            animator.transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            animator.transform.localScale = new Vector3(-1, 1, 1);

        if (Vector2.Distance(transform.position, target) < reachDistance && waitCoroutine == null)
        {
            waitCoroutine = StartCoroutine(WaitAtPoint()); // ← 防止重复启动
        }
    }

    public void StopPatrol()
    {
        isStopped = true; // ← 用独立标志，不影响 isWaiting
        rb.velocity = Vector2.zero;
        animator.Play("Idle");
    }

    public void ResumePatrol()
    {
        isStopped = false;
        if (hasStarted && !isWaiting && gameObject.activeInHierarchy)
            animator.Play("Walk");
    }
    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        rb.velocity = Vector2.zero;
        animator.Play("Idle");
        yield return new WaitForSeconds(waitTime);
        currentIndex = (currentIndex + 1) % patrolPoints.Length;
        isWaiting = false;
        waitCoroutine = null; // ← 协程结束后清空
        if (!isStopped)
            animator.Play("Walk");
    }
}