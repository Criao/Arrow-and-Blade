using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var results = new List<RaycastResult>();
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"=== 点击命中 {results.Count} 个对象 ===");
            foreach (var r in results)
            {
                Debug.Log($"命中: {r.gameObject.name} | 父级: {r.gameObject.transform.parent?.name}");
            }
        }
    }
}