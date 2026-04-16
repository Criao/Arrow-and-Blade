using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] itemSlots;
    [SerializeField] private UseItem useItem;
    public int gold;
    public TMP_Text goldText;
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private Transform player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        goldText.text = gold.ToString();
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }


    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }
    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSo itemSo, int quantity)
    {
        if (itemSo.isGold)
        {
            AddGold(quantity);
            return;
        }
        foreach (var slot in itemSlots)
        {
            if (slot.itemSo == itemSo && slot.quantity < itemSo.StackSize)
            {
                int availableSpace = itemSo.StackSize - slot.quantity;
                int amountToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                slot.UpdateUI();

                if (quantity <= 0)
                    return;
            }
        }
        foreach (var slot in itemSlots)
        {
            if (slot.itemSo == null)
            {
                int amountToAdd = Mathf.Min(itemSo.StackSize,quantity);
                slot.itemSo = itemSo;
                slot.quantity = quantity;
                slot.UpdateUI();
                return;
            }
        }
        if(quantity > 0)
        {
            DropLoot(itemSo,quantity);
        }
    }

    /// <summary>
    /// 添加金币
    /// </summary>
    public void AddGold(int amount)
    {
        gold += amount;
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
        Debug.Log($"金币增加 {amount}，当前金币: {gold}");
    }
    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSo,1);
        slot.quantity--;
        if(slot.quantity <= 0)
        {
            slot.itemSo = null;
        }
        slot.UpdateUI();
    }
    private void DropLoot(ItemSo itemSo,int quantity)
    {
        Loot loot = Instantiate(lootPrefab,player.position,Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSo,quantity);
    }
    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSo != null && slot.quantity > 0)
        {
            useItem.ApplyItemEffects(slot.itemSo);
            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSo = null;
            }
            slot.UpdateUI();
        }
    }
}
