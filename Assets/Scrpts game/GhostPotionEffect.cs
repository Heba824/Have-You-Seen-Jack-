using System.Collections;
using UnityEngine;

public class GhostPotionEffect : MonoBehaviour
{
    [Header("Material & Settings")]
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private float ghostDuration = 8f;

    [Header("VFX Prefab")]
    [SerializeField] private GameObject ghostVFXPrefab;

    public bool isGhost { get; private set; }

    private Renderer[] witchRenderers;
    private Material[][] originalMaterials;
    private int originalLayer;

    private void Awake()
    {
        witchRenderers = GetComponentsInChildren<Renderer>(true);
        originalLayer = gameObject.layer;
    }
    
    public void ActivateGhostMode()
    {
        if (!isGhost)
        {
            StartCoroutine(GhostRoutine());
        }
    }

    private IEnumerator GhostRoutine()
    {
        isGhost = true;

        // 1. Cache all current materials across all character sub-meshes
        SaveOriginalMaterials();
        originalLayer = gameObject.layer;

        // 2. Set Layer to GhostPlayer & apply ghost material to all sub-meshes
        gameObject.layer = LayerMask.NameToLayer("GhostPlayer");
        ApplyGhostMaterial();
        SpawnVFX();

        yield return new WaitForSeconds(ghostDuration);

        // 3. Restore all original materials & reset layer
        RestoreOriginalMaterials();
        gameObject.layer = originalLayer;
        SpawnVFX();

        isGhost = false;
    }

    private void SaveOriginalMaterials()
    {
        originalMaterials = new Material[witchRenderers.Length][];
        for (int i = 0; i < witchRenderers.Length; i++)
        {
            originalMaterials[i] = witchRenderers[i].sharedMaterials;
        }
    }

    private void ApplyGhostMaterial()
    {
        if (ghostMaterial == null) return;

        foreach (Renderer rend in witchRenderers)
        {
            // Skip pumpkin mesh if present
            if (rend.gameObject.name.Contains("Pumpkin")) continue;

            Material[] ghostMatArray = new Material[rend.sharedMaterials.Length];
            for (int m = 0; m < ghostMatArray.Length; m++)
            {
                ghostMatArray[m] = ghostMaterial;
            }
            rend.materials = ghostMatArray;
        }
    }

    private void RestoreOriginalMaterials()
    {
        if (originalMaterials == null) return;

        for (int i = 0; i < witchRenderers.Length; i++)
        {
            if (witchRenderers[i] != null && originalMaterials[i] != null)
            {
                witchRenderers[i].materials = originalMaterials[i];
            }
        }
    }

    private void SpawnVFX()
    {
        if (ghostVFXPrefab != null)
        {
            GameObject vfx = Instantiate(ghostVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }
}