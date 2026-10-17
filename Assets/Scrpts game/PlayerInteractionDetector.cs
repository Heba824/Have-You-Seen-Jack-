using UnityEngine;
using TMPro;

public class PlayerInteractionDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactRange = 3.0f;
    public LayerMask interactableLayer;
    public Transform cameraTransform;

    [Header("UI Reference")]
    public GameObject promptPanel;     // Drag PromptPanel here (parent object containing Panel, Badge, & Text)
    public TextMeshProUGUI promptText; // Drag PromptText here

    private IInteractable currentInteractable;

    void Update()
    {
        DetectInteractable();

        // Press E to interact
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    private void DetectInteractable()
    {
        // Safety check to avoid NullReference error if camera isn't assigned
        if (cameraTransform == null) return;

        // 1. Raycast check forward from camera center
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            // Use GetComponentInParent to catch IInteractable scripts on parent GameObjects
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                SetCurrentInteractable(interactable);
                return;
            }
        }

        // 2. OverlapSphere check around player position
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
        foreach (var col in hitColliders)
        {
            IInteractable interactable = col.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                SetCurrentInteractable(interactable);
                return;
            }
        }

        // 3. Clear UI if nothing is found
        ClearInteractable();
    }

    private void SetCurrentInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;

        if (promptText != null)
        {
            promptText.text = interactable.GetInteractPrompt();
        }

        if (promptPanel != null)
        {
            promptPanel.SetActive(true); // ✅ Enables PromptPanel + KeyBadge + PromptText together
        }
    }

    private void ClearInteractable()
    {
        currentInteractable = null;

        if (promptPanel != null)
        {
            promptPanel.SetActive(false); // ✅ Disables the entire UI panel
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}