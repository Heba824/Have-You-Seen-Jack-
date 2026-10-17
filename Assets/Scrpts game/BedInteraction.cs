using UnityEngine;

public class BedInteraction : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public Transform sleepPosition;     // Target transform on top of the bed

    private bool isSleeping = false;

    private GameObject player;
    private PlayerMovementWitch playerMovement;
    private CharacterController controller;
    private Animator animator;

    private Vector3 originalPlayerPos;
    private Quaternion originalPlayerRot;

    void Update()
    {
        // Continuously lock player position & rotation while sleeping
        if (isSleeping && sleepPosition != null && player != null)
        {
            player.transform.position = sleepPosition.position;
            player.transform.rotation = sleepPosition.rotation;
        }
    }

    public string GetInteractPrompt()
    {
        return isSleeping ? "Wake Up" : "Sleep";
    }

    public void Interact()
    {
        // Cache references dynamically if they haven't been assigned yet
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMovement = player.GetComponent<PlayerMovementWitch>();
                controller = player.GetComponent<CharacterController>();
                animator = player.GetComponentInChildren<Animator>();
            }
        }

        if (player == null)
        {
            Debug.LogError("BedInteraction: No object tagged 'Player' found in scene!");
            return;
        }

        if (!isSleeping)
        {
            Sleep();
        }
        else
        {
            WakeUp();
        }
    }

    private void Sleep()
    {
        isSleeping = true;

        // Store original position to restore when waking up
        originalPlayerPos = player.transform.position;
        originalPlayerRot = player.transform.rotation;

        // Disable movement and character controller physics during sleep
        if (playerMovement != null) playerMovement.enabled = false;
        if (controller != null) controller.enabled = false;

        // Move and rotate character to sleep spot
        if (sleepPosition != null)
        {
            player.transform.position = sleepPosition.position;
            player.transform.rotation = sleepPosition.rotation;
        }

        // Trigger Animator parameter
        if (animator != null)
        {
            animator.SetBool("isSleeping", true);
        }
        else
        {
            Debug.LogWarning("BedInteraction: Animator component missing on Player or its children!");
        }

        Debug.Log("Avatar is sleeping. Press E to wake up.");
    }

    private void WakeUp()
    {
        isSleeping = false;

        // Reset animation
        if (animator != null)
        {
            animator.SetBool("isSleeping", false);
        }

        // Restore position beside bed
        player.transform.position = originalPlayerPos;
        player.transform.rotation = originalPlayerRot;

        // Re-enable movement and controller physics
        if (controller != null) controller.enabled = true;
        if (playerMovement != null) playerMovement.enabled = true;

        Debug.Log("Avatar woke up.");
    }
}