using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Transform itemsParent; // Grid Layout container
    [SerializeField] private GameObject inventorySlotPrefab; // Slot Prefab

    void Start()
    {
        // Auto-find player inventory if not assigned
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<PlayerInventory>();
            }
        }
    }

    void Update()
    {
        // Continuously refresh UI
        RefreshUI();
    }

    // Changed to public void RefreshUI() to resolve CS1061 error
    public void RefreshUI()
    {
        if (playerInventory == null || itemsParent == null || inventorySlotPrefab == null) return;

        // 1. Clear existing slot instances
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Instantiate a UI slot for each collected item
        foreach (ItemData item in playerInventory.items)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, itemsParent);
            
            // Set Icon
            Image iconImage = slotObj.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                if (item.itemIcon != null)
                {
                    iconImage.gameObject.SetActive(true);
                    iconImage.sprite = item.itemIcon;
                }
                else
                {
                    iconImage.gameObject.SetActive(false);
                }
            }

            // Set Text/Name
            TMP_NameCheck(slotObj, item.itemName);
        }
    }

    private void TMP_NameCheck(GameObject slotObj, string itemName)
    {
        TextMeshProUGUI tmpText = slotObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = itemName;
            return;
        }

        Text legacyText = slotObj.GetComponentInChildren<Text>();
        if (legacyText != null)
        {
            legacyText.text = itemName;
        }
    }
}