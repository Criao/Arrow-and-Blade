using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 进化区域出口，玩家离开时恢复碰撞体和渲染层级
/// </summary>
public class Evolution_Exit : MonoBehaviour
{
    [SerializeField] private Collider2D[] mountainColliders; // 山体碰撞体数组
    [SerializeField] private Collider2D[] boundaryColliders; // 边界碰撞体数组

    /// <summary>
    /// 玩家进入触发器时启用山体碰撞，禁用边界碰撞，恢复渲染层级
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 启用山体碰撞体，恢复正常碰撞
            foreach (Collider2D mountain in mountainColliders)
            {
                mountain.enabled = true;
            }
            // 禁用边界碰撞体
            foreach (Collider2D boundary in boundaryColliders)
            {
                boundary.enabled = false;
            }
            // 恢复玩家渲染层级
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
    }
}
