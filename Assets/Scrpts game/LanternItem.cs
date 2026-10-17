using UnityEngine;

public class LanternItem : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    private bool isPlayerInRange = false;
    private bool isHeld = false;

    [Header("Hand Offset Settings")]
    public Vector3 holdPositionOffset = new Vector3(0f, -0.15f, 0.1f);
    public Vector3 holdRotationOffset = new Vector3(0f, 0f, 0f);

    [Header("Drop Settings")]
    public float dropForwardDistance = 0.8f;
    public float dropHeightOffset = 0.2f;

    private Transform playerHandSocket;
    private Transform playerTransform;
    private Animator playerAnimator;

    void Update()
    {
        // Pick up lantern with E
        if (isPlayerInRange && !isHeld && Input.GetKeyDown(KeyCode.E))
        {
            PickupLantern();
        }
        // Drop lantern with F
        else if (isHeld && Input.GetKeyDown(KeyCode.F))
        {
            DropLantern();
        }
    }

    private void PickupLantern()
    {
        isHeld = true;

        // Parent lantern to hand socket
        transform.SetParent(playerHandSocket);

        // Apply local offsets
        transform.localPosition = holdPositionOffset;
        transform.localRotation = Quaternion.Euler(holdRotationOffset);

        // Disable physics/colliders
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsHoldingLantern", true);
        }

        Debug.Log("Lantern picked up! Press F to put down.");
    }

    private void DropLantern()
    {
        isHeld = false;
        transform.SetParent(null);

        // Target spot in front of player
        Vector3 targetSpot = playerTransform.position + (playerTransform.forward * dropForwardDistance) + (Vector3.up * 1.0f);

        // Raycast down to find exact floor height
        if (Physics.Raycast(targetSpot, Vector3.down, out RaycastHit hit, 3.0f))
        {
            transform.position = hit.point + (Vector3.up * dropHeightOffset);
        }
        else
        {
            transform.position = targetSpot;
        }

        transform.rotation = Quaternion.LookRotation(playerTransform.forward);

        // Re-enable colliders & physics
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsHoldingLantern", false);
        }

        Debug.Log("Lantern put down.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
            playerAnimator = other.GetComponentInChildren<Animator>();

            if (playerHandSocket == null)
            {
                playerHandSocket = FindChildRecursive(other.transform, "LanternSocket");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindChildRecursive(child, name);
            if (result != null) return result;
        }
        return null;
    }
    public string GetInteractPrompt()
    {
        return " Pick Up Lantern";
    }

    public void Interact()
    {
        // Call your existing lantern pick function!
        PickupLantern(); 
    }
}
