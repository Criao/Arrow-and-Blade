using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭矢对象池，用于管理和复用箭矢对象，提高性能
/// </summary>
public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance { get; private set; }

    [SerializeField] private GameObject arrowPrefab; // 箭矢预制体
    [SerializeField] private int poolSize = 8; // 对象池初始大小

    private Queue<Arrow> availableArrows = new Queue<Arrow>(); // 可用箭矢队列

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// 初始化对象池，预先创建指定数量的箭矢
    /// </summary>
    private void InitializePool()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowPool is missing an arrow prefab.");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrowObj = Instantiate(arrowPrefab);
            arrowObj.SetActive(false);
            Arrow arrow = arrowObj.GetComponent<Arrow>();
            if (arrow != null)
            {
                availableArrows.Enqueue(arrow);
            }
            else
            {
                Destroy(arrowObj);
            }
        }
    }

    /// <summary>
    /// 从对象池获取箭矢
    /// </summary>
    public Arrow GetArrow(Vector3 position, Vector2 direction)
    {
        Arrow arrow = null;

        // 从队列中获取可用箭矢
        while (availableArrows.Count > 0)
        {
            arrow = availableArrows.Dequeue();
            if (arrow != null)
            {
                break;
            }
        }

        // 如果对象池耗尽，动态创建新箭矢
        if (arrow == null)
        {
            if (arrowPrefab == null)
            {
                Debug.LogError("Cannot create an arrow because ArrowPool is missing an arrow prefab.");
                return null;
            }

            Debug.LogWarning("对象池耗尽或引用失效，动态创建箭矢");
            GameObject arrowObj = Instantiate(arrowPrefab);
            arrowObj.SetActive(false);
            arrow = arrowObj.GetComponent<Arrow>();
        }

        if (arrow == null)
        {
            Debug.LogError("Cannot get an Arrow component from the arrow prefab.");
            return null;
        }

        arrow.transform.position = position;
        arrow.gameObject.SetActive(true);
        arrow.Initialize(direction);

        return arrow;
    }

    /// <summary>
    /// 将箭矢返回对象池
    /// </summary>
    public void ReturnArrow(Arrow arrow)
    {
        if (arrow != null && arrow.gameObject != null)
        {
            arrow.gameObject.SetActive(false);
            availableArrows.Enqueue(arrow);
        }
    }
}
