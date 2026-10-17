using System.Collections;
using UnityEngine;

public class PumpkinCamouflageEffect : MonoBehaviour
{
    [Header("Mesh References")]
    [SerializeField] private GameObject pumpkinMesh;

    [Header("VFX Prefab (Option 2)")]
    [SerializeField] private GameObject poofVFXPrefab; // Drag the project asset here

    [Header("Gliding & Wobble Settings")]
    [SerializeField] private float tiltAngle = 12f;
    [SerializeField] private float wobbleSpeed = 8f;
    [SerializeField] private float duration = 10f;

    public bool isCamouflaged { get; private set; }

    private Renderer[] witchRenderers;
    private Quaternion originalPumpkinRotation;

    private void Awake()
    {
        witchRenderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        if (pumpkinMesh != null)
        {
            originalPumpkinRotation = pumpkinMesh.transform.localRotation;
            pumpkinMesh.SetActive(false);
        }
    }

    public void ActivateCamouflage()
    {
        if (!isCamouflaged)
        {
            StartCoroutine(CamoRoutine());
        }
    }

    private IEnumerator CamoRoutine()
    {
        isCamouflaged = true;

        // 1. Spawn poof effect on transformation
        SpawnPoofVFX();

        // 2. Hide witch meshes & show pumpkin
        SetWitchVisibility(false);
        if (pumpkinMesh != null) pumpkinMesh.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            HandlePumpkinGlidingMotion();
            yield return null;
        }

        // 3. Reset pumpkin rotation & hide
        if (pumpkinMesh != null)
        {
            pumpkinMesh.transform.localRotation = originalPumpkinRotation;
            pumpkinMesh.SetActive(false);
        }

        // 4. Show witch & spawn return poof effect
        SetWitchVisibility(true);
        SpawnPoofVFX();

        isCamouflaged = false;
    }

    private void SpawnPoofVFX()
    {
        if (poofVFXPrefab != null)
        {
            // Instantiate a fresh particle burst at player position
            GameObject spawnedVFX = Instantiate(poofVFXPrefab, transform.position, Quaternion.identity);


            // Automatically destroy it after 0.35 seconds to clean up memory
            Destroy(spawnedVFX, 0.35f);
        }
    }

    private void SetWitchVisibility(bool visible)
    {
        if (witchRenderers == null) return;

        foreach (Renderer rend in witchRenderers)
        {
            if (pumpkinMesh != null && rend.gameObject == pumpkinMesh) continue;
            if (pumpkinMesh != null && rend.transform.IsChildOf(pumpkinMesh.transform)) continue;

            rend.enabled = visible;
        }
    }

    private void HandlePumpkinGlidingMotion()
    {
        if (pumpkinMesh == null) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool isMoving = (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f);

        if (isMoving)
        {
            float zTilt = -horizontalInput * tiltAngle;
            float yWobble = Mathf.Sin(Time.time * wobbleSpeed) * 5f;

            Quaternion targetRotation = originalPumpkinRotation * Quaternion.Euler(0f, yWobble, zTilt);
            pumpkinMesh.transform.localRotation = Quaternion.Slerp(pumpkinMesh.transform.localRotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            pumpkinMesh.transform.localRotation = Quaternion.Slerp(pumpkinMesh.transform.localRotation, originalPumpkinRotation, Time.deltaTime * 8f);
        }
    }
}