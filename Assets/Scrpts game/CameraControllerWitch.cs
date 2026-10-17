using UnityEngine;
using Unity.Cinemachine;

public class CameraControllerWitch : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera thirdPersonCam;
    [SerializeField] private CinemachineCamera firstPersonCam;

    [Header("Controls & Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.V;
    [SerializeField] private PlayerMovementWitch playerMovement;

    [Header("Player Mesh Settings")]
[SerializeField] private SkinnedMeshRenderer playerMesh; 
    private bool isFirstPerson = false;

    private void Start()
    {
        // Lock cursor to center of screen for smooth camera orbiting
        LockCursor();

        // Set initial camera priorities
        SetCameraState(false);
    }

    private void Update()
    {
        // Toggle between First-Person and Third-Person views
        if (Input.GetKeyDown(toggleKey))
        {
            isFirstPerson = !isFirstPerson;
            SetCameraState(isFirstPerson);
        }

        // Press Escape to unlock mouse cursor during play mode
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        // Click on game screen to lock cursor back in
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }
    }
// Drag your character's SkinnedMeshRenderer here

private void SetCameraState(bool fpActive)
{
    if (thirdPersonCam == null || firstPersonCam == null) return;

    if (fpActive)
    {
        firstPersonCam.Priority = 20;
        thirdPersonCam.Priority = 10;

        // Hide mesh in 1st person so shoulders/head don't cover the screen
        if (playerMesh != null) 
            playerMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }
    else
    {
        thirdPersonCam.Priority = 20;
        firstPersonCam.Priority = 10;

        // Re-enable mesh visibility in 3rd person
        if (playerMesh != null) 
            playerMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    if (playerMovement != null)
    {
        playerMovement.SetFirstPersonMode(fpActive);
    }
}


    

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
}