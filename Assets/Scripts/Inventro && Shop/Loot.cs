using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.U2D;

/// <summary>
/// 掉落物类，处理地面上可拾取物品的逻辑
/// </summary>
public class Loot : MonoBehaviour
{
    [SerializeField] private ItemSo itemSo; // 掉落的物品
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;
    [SerializeField] private bool canBePickedUp = true; // 是否可以被拾取
    [SerializeField] private int quantity; // 物品数量
    public static event Action<ItemSo, int> OnItemLooted; // 物品被拾取事件

    private void OnValidate()
    {
        if (itemSo == null)
            return;
        this.name = itemSo.ItemName;
    }

    private void Start()
    {
        if (itemSo != null)
        {
            UpdateAppearance();
        }
    }

    /// <summary>
    /// 初始化掉落物
    /// </summary>
    public void Initialize(ItemSo itemSo, int quantity)
    {
        this.itemSo = itemSo;
        this.quantity = quantity;
        canBePickedUp = false;
        UpdateAppearance();
        StartCoroutine(EnablePickupAfterDelay(0.5f)); // 延时后允许拾取，防止立即拾取
    }

    /// <summary>
    /// 延迟启用拾取功能
    /// </summary>
    private IEnumerator EnablePickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canBePickedUp = true;
    }

    /// <summary>
    /// 更新掉落物外观
    /// </summary>
    private void UpdateAppearance()
    {
        if (itemSo == null)
        {
            return;
        }

        if (sr != null)
        {
            sr.sprite = itemSo.Icon;
        }

        this.name = itemSo.ItemName;
    }

    /// <summary>
    /// 玩家触碰时拾取物品
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !canBePickedUp || itemSo == null || quantity <= 0)
        {
            return;
        }

        canBePickedUp = false;
        HitFeedbackAudio.PlayLightHitConfirm(transform.position);

        if (anim != null)
        {
            anim.Play("LootPickUp");
        }

        OnItemLooted?.Invoke(itemSo, quantity);
        Destroy(gameObject, 0.5f);
    }
}
