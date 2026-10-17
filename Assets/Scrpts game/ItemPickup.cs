using System.Collections;

using UnityEngine;



public class ItemPickup : MonoBehaviour, IInteractable

{




[Header("Item Settings")]



public ItemData itemData;

private bool isPlayerInRange = false;

private PlayerInventory playerInventory;

private bool isPickedUp = false;



void Update()

{

if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isPickedUp)

{

Pickup();

}

}



private void Pickup()

{

if (playerInventory != null)

{

isPickedUp = true;

playerInventory.AddItem(itemData);



// Disable trigger/colliders so it can't be interacted with again

Collider col = GetComponent<Collider>();

if (col != null) col.enabled = false;



// Start the parenting and timing process

StartCoroutine(AttachToHandAndDestroy());

}

}



private IEnumerator AttachToHandAndDestroy()

{

// Optional: Wait for the bending-down animation frame where hand reaches ground

yield return new WaitForSeconds(0.3f);



if (playerInventory.handHoldPoint != null)

{

// Parent to hand bone

transform.SetParent(playerInventory.handHoldPoint);



// Reset local position & rotation to sit nicely in the palm

transform.localPosition = Vector3.zero;

transform.localRotation = Quaternion.identity;

}



// Wait for the arm to reach the body before vanishing into inventory

yield return new WaitForSeconds(0.7f);



Destroy(gameObject);

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

public string GetInteractPrompt()

{

return $"Pick Up {itemData.itemName}";

}



public void Interact()

{

// Call your existing pickup function!

Pickup();

}

} 