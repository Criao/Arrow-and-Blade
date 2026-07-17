using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance { get; private set; }

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int poolSize = 8;

    private readonly Queue<Arrow> availableArrows = new Queue<Arrow>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePool();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void InitializePool()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowPool is missing an arrow prefab.");
            return;
        }

        for (int i = 0; i < Mathf.Max(0, poolSize); i++)
        {
            Arrow arrow = CreateArrow();
            if (arrow != null)
            {
                availableArrows.Enqueue(arrow);
            }
        }
    }

    public Arrow GetArrow(Vector3 position, Vector2 direction)
    {
        Arrow arrow = GetAvailableArrow();
        if (arrow == null)
        {
            arrow = CreateArrow();
        }

        if (arrow == null)
        {
            Debug.LogError("Cannot get an arrow because ArrowPool is missing a valid arrow prefab.");
            return null;
        }

        arrow.transform.SetParent(null);
        arrow.transform.position = position;
        arrow.gameObject.SetActive(true);
        arrow.Initialize(direction);

        return arrow;
    }

    public void ReturnArrow(Arrow arrow)
    {
        if (arrow == null || arrow.gameObject == null)
        {
            return;
        }

        arrow.transform.SetParent(transform);
        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.identity;
        arrow.gameObject.SetActive(false);

        if (!availableArrows.Contains(arrow))
        {
            availableArrows.Enqueue(arrow);
        }
    }

    private Arrow GetAvailableArrow()
    {
        while (availableArrows.Count > 0)
        {
            Arrow arrow = availableArrows.Dequeue();
            if (arrow != null)
            {
                return arrow;
            }
        }

        return null;
    }

    private Arrow CreateArrow()
    {
        if (arrowPrefab == null)
        {
            return null;
        }

        GameObject arrowObj = Instantiate(arrowPrefab, transform);
        arrowObj.SetActive(false);

        Arrow arrow = arrowObj.GetComponent<Arrow>();
        if (arrow == null)
        {
            Debug.LogError("Cannot get an Arrow component from the arrow prefab.");
            Destroy(arrowObj);
        }

        return arrow;
    }
}
