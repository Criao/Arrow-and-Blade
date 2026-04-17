using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance { get; private set; }

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int poolSize = 8;

    private Queue<Arrow> availableArrows = new Queue<Arrow>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrowObj = Instantiate(arrowPrefab);
            arrowObj.SetActive(false);
            Arrow arrow = arrowObj.GetComponent<Arrow>();
            availableArrows.Enqueue(arrow);
        }
    }

    public Arrow GetArrow(Vector3 position, Vector2 direction)
    {
        Arrow arrow = null;

        while (availableArrows.Count > 0)
        {
            arrow = availableArrows.Dequeue();
            if (arrow != null)
            {
                break;
            }
        }

        if (arrow == null)
        {
            Debug.LogWarning("对象池耗尽或引用失效，动态创建箭矢");
            GameObject arrowObj = Instantiate(arrowPrefab);
            arrowObj.SetActive(false);
            arrow = arrowObj.GetComponent<Arrow>();
        }

        arrow.transform.position = position;
        arrow.gameObject.SetActive(true);
        arrow.Initialize(direction);

        return arrow;
    }

    public void ReturnArrow(Arrow arrow)
    {
        if (arrow != null && arrow.gameObject != null)
        {
            arrow.gameObject.SetActive(false);
            availableArrows.Enqueue(arrow);
        }
    }
}
