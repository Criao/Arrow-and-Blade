using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.U2D;

public class Loot : MonoBehaviour
{
    [SerializeField] private ItemSo itemSo;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;
    [SerializeField] private bool canBePickedUp = true;
    [SerializeField] private int quantity;
    public static event Action<ItemSo, int> OnItemLooted;

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
    public void Initialize(ItemSo itemSo, int quantity)
    {
        this.itemSo = itemSo;
        this.quantity = quantity;
        canBePickedUp = false;
        UpdateAppearance();
        StartCoroutine(EnablePickupAfterDelay(0.5f)); // 延时后允许拾取
    }

    private IEnumerator EnablePickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canBePickedUp = true;
    }
    private void UpdateAppearance()
    {
        sr.sprite = itemSo.Icon;
        this.name = itemSo.ItemName;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && canBePickedUp == true)
        {
            anim.Play("LootPickUp");
            OnItemLooted?.Invoke(itemSo, quantity);
            Destroy(gameObject, 0.5f);
        }
    }
}
