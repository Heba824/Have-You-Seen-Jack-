using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button useButton;

    private ItemData assignedItem;
    private PlayerInventory playerInventory;
    private InventoryUI inventoryUI;

    public void Setup(ItemData item, PlayerInventory inventory, InventoryUI ui)
    {
        assignedItem = item;
        playerInventory = inventory;
        inventoryUI = ui;

        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }

        if (nameText != null)
        {
            nameText.text = item.itemName;
        }

        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnUseClicked);
        }
    }

    private void OnUseClicked()
    {
        if (playerInventory != null && assignedItem != null)
        {
            playerInventory.UseItem(assignedItem);

            if (inventoryUI != null)
            {
                inventoryUI.RefreshUI();
            }
        }
    }
}