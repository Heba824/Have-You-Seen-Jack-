using UnityEngine;

public class GateTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator gateAnimator; // Drag the parent object here in Inspector
    private bool isOpen = false;

    void Start()
    {
        if (gateAnimator == null)
            gateAnimator = GetComponentInParent<Animator>();
    }

    // Dynamic prompt text that changes based on the gate's state
    public string GetInteractPrompt()
    {
        return isOpen ? "Close Door" : "Open Door";
    }

    // Triggered automatically by PlayerInteractionDetector when pressing E
    public void Interact()
    {
        isOpen = !isOpen;

        if (gateAnimator != null)
        {
            gateAnimator.SetBool("IsOpen", isOpen);
        }
    }
}