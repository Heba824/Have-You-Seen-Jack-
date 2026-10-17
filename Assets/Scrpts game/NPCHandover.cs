using UnityEngine;

public class NPCHandover : MonoBehaviour
{
    public ItemData requiredItem; // Assign 'CandyBar_Data' in Inspector
    private bool isPlayerInRange = false;
    private PlayerInventory playerInventory;
    private bool itemReceived = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !itemReceived)
        {
            TryReceiveItem();
        }
    }

    private void TryReceiveItem()
    {
        if (playerInventory != null && playerInventory.HasItem(requiredItem))
        {
            playerInventory.RemoveItem(requiredItem);
            itemReceived = true;
            
            // Trigger NPC response (e.g., animation, dialogue, unlock door)
            Debug.Log("NPC says: Thank you for the " + requiredItem.itemName + "!");
        }
        else
        {
            Debug.Log("NPC says: Hey, I need a " + requiredItem.itemName + "!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerInventory = null;
        }
    }
}