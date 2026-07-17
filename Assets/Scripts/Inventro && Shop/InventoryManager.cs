using TMPro;
using UnityEngine;

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
        UpdateGoldUI();

        if (itemSlots == null)
        {
            return;
        }

        foreach (InventorySlot slot in itemSlots)
        {
            if (slot != null)
            {
                slot.UpdateUI();
            }
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
        if (itemSo == null || quantity <= 0)
        {
            return;
        }

        if (itemSo.isGold)
        {
            AddGold(quantity);
            return;
        }

        AddToExistingStacks(itemSo, ref quantity);
        AddToEmptySlots(itemSo, ref quantity);

        if (quantity > 0)
        {
            DropLoot(itemSo, quantity);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
        Debug.Log($"Gold changed by {amount}. Current gold: {gold}");
    }

    public void DropItem(InventorySlot slot)
    {
        if (slot == null || slot.itemSo == null || slot.quantity <= 0)
        {
            return;
        }

        DropLoot(slot.itemSo, 1);
        slot.quantity--;

        if (slot.quantity <= 0)
        {
            slot.itemSo = null;
        }

        slot.UpdateUI();
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot == null || slot.itemSo == null || slot.quantity <= 0 || useItem == null)
        {
            return;
        }

        useItem.ApplyItemEffects(slot.itemSo);
        slot.quantity--;

        if (slot.quantity <= 0)
        {
            slot.itemSo = null;
        }

        slot.UpdateUI();
    }

    private void AddToExistingStacks(ItemSo itemSo, ref int quantity)
    {
        if (itemSlots == null)
        {
            return;
        }

        int stackSize = Mathf.Max(1, itemSo.StackSize);
        foreach (InventorySlot slot in itemSlots)
        {
            if (slot == null || slot.itemSo != itemSo || slot.quantity >= stackSize)
            {
                continue;
            }

            int availableSpace = stackSize - slot.quantity;
            int amountToAdd = Mathf.Min(availableSpace, quantity);
            slot.quantity += amountToAdd;
            quantity -= amountToAdd;
            slot.UpdateUI();

            if (quantity <= 0)
            {
                return;
            }
        }
    }

    private void AddToEmptySlots(ItemSo itemSo, ref int quantity)
    {
        if (itemSlots == null)
        {
            return;
        }

        int stackSize = Mathf.Max(1, itemSo.StackSize);
        foreach (InventorySlot slot in itemSlots)
        {
            if (slot == null || slot.itemSo != null)
            {
                continue;
            }

            int amountToAdd = Mathf.Min(stackSize, quantity);
            slot.itemSo = itemSo;
            slot.quantity = amountToAdd;
            quantity -= amountToAdd;
            slot.UpdateUI();

            if (quantity <= 0)
            {
                return;
            }
        }
    }

    private void DropLoot(ItemSo itemSo, int quantity)
    {
        if (itemSo == null || lootPrefab == null || player == null || quantity <= 0)
        {
            Debug.LogWarning("Cannot drop loot because item, prefab, player, or quantity is invalid.");
            return;
        }

        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        if (loot != null)
        {
            loot.Initialize(itemSo, quantity);
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }
}
