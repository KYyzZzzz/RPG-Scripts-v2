using System;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] itemSlots;
    public UseItem useItem;
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
    public Transform player;

    public static event Action<int> OnExperienceGained;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable()
    {
        LOOT.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        LOOT.OnItemLooted -= AddItem;
    }


    public void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }

        if (itemSO.isEXP)
        {
            OnExperienceGained?.Invoke(quantity);
            return;
        }



        foreach (var slot in itemSlots)
        {
            if(slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                int availableSpace = itemSO.stackSize - slot.quantity;
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
            if (slot.itemSO == null)
            {
                int amountToAdd = Mathf.Min(itemSO.stackSize,  quantity);
                slot.itemSO = itemSO;
                slot.quantity = quantity;
                slot.UpdateUI();
                return;
            }

        }

        if (quantity > 0)
            DropLoot(itemSO, quantity);
    }



    public void RemoveItem(ItemSO itemSO, int quantity)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slot = itemSlots[i];

            //Skip slots htat don't match the item
            if (slot.itemSO != itemSO)
                continue;

            if (slot.quantity > quantity)
            {
                //Remove only what we need
                slot.quantity -= quantity;
                slot.UpdateUI();
                quantity = 0;
            }


            else
            {
                //take ALL from this slot
                quantity -= slot.quantity;
                slot.itemSO = null;
                slot.quantity = 0;
                slot.UpdateUI();
            }
        }
    }




    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }



    private void DropLoot(ItemSO itemSO, int quantity)
    {
        LOOT loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<LOOT>();
        loot.Initialize(itemSO, quantity);
    }


    public void UseItem(InventorySlot slot)
    {
        // Cari ulang referensi kalau sempat hilang akibat reload scene
        if (useItem == null)
            useItem = FindFirstObjectByType<UseItem>();

        if (slot.itemSO != null && slot.quantity >= 0 && useItem != null)
        {
            useItem.ApplyItemEffects(slot.itemSO);

            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }


    public bool HasItem(ItemSO itemSO)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemSO == itemSO && slot.quantity > 0)
                return true;
        }
        return false;
    }


    public int GetItemQuantity(ItemSO itemSO)
    {
        int total = 0;

        foreach (var slot in itemSlots)
        {
            if (slot.itemSO == itemSO)
                total += slot.quantity;
        }

        return total;
    }
}
