using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cinemachine相机边界查找器，自动查找场景中的Confiner边界
/// </summary>
public class ConfinerFinder : MonoBehaviour
{
    private CinemachineConfiner2D confiner; // Cinemachine边界组件

    /// <summary>
    /// 初始化，缓存组件
    /// </summary>
    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    /// <summary>
    /// 启用时监听场景加载事件
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 禁用时取消监听
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载完成时查找并设置边界
    /// </summary>
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
        confiner.InvalidateCache();
    }
}