using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class ConfinerFinder : MonoBehaviour
{
    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        // 提前缓存组件，避免重复 GetComponent
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()  // ✅ 修复拼写错误
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject confinerObj = GameObject.FindWithTag("Confiner");

        if (confinerObj == null)
        {
            Debug.LogWarning($"场景 [{scene.name}] 中未找到 Tag 为 'Confiner' 的物体！");
            return;
        }

        PolygonCollider2D polygon = confinerObj.GetComponent<PolygonCollider2D>();

        if (polygon == null)
        {
            Debug.LogWarning($"找到了 Confiner 物体，但没有 PolygonCollider2D 组件！");
            return;
        }

        confiner.m_BoundingShape2D = polygon;
        confiner.InvalidateCache(); // ✅ 关键：清除旧边界缓存，强制重新计算
    }
}