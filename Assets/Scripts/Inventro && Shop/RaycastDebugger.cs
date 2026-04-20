using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Raycast调试工具，用于调试UI点击事件
/// </summary>
public class RaycastDebugger : MonoBehaviour
{
    /// <summary>
    /// 每帧检测鼠标点击，输出射线检测结果
    /// </summary>
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