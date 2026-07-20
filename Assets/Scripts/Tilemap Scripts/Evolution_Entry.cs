using UnityEngine;

/// <summary>
/// 进化区域入口，玩家进入时切换碰撞体和渲染层级
/// </summary>
public class Evolution_Entry : MonoBehaviour
{
    [SerializeField] private Collider2D[] mountainColliders; // 山体碰撞体数组
    [SerializeField] private Collider2D[] boundaryColliders; // 边界碰撞体数组

    /// <summary>
    /// 玩家进入触发器时禁用山体碰撞，启用边界碰撞，提升渲染层级
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 禁用山体碰撞体，允许玩家穿过
            foreach (Collider2D mountain in mountainColliders)
            {
                mountain.enabled = false;
            }
            // 启用边界碰撞体，限制玩家活动范围
            foreach (Collider2D boundary in boundaryColliders)
            {
                boundary.enabled = true;
            }
            // 提升玩家渲染层级，显示在山体前面
            SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 15;
            }
        }
    }
}
