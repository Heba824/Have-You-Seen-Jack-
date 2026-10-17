using UnityEngine;

public class InteractableLight : MonoBehaviour, IInteractable
{
    [Header("Light References")]
    [SerializeField] private Light targetLight; // The PointLight or SpotLight inside/near the lantern
    [SerializeField] private MeshRenderer lanternMeshRenderer;
    [SerializeField] private int materialIndex = 1; // Which material index on the mesh uses the emission/glow

    [Header("Lighting Settings")]
    public bool isOn = false;
    public float onIntensity = 2.0f;
    public Color warmLightColor = new Color(1.0f, 0.65f, 0.2f); // Cozy amber

    [Header("Interaction Prompt")]
    [SerializeField] private string lightName = "Lantern";

    void Start()
    {
        // Auto-fetch light if not assigned
        if (targetLight == null)
            targetLight = GetComponentInChildren<Light>();

        // Set initial state
        UpdateLightState();
    }

    // --- IInteractable Implementation ---
    public void Interact()
    {
        ToggleLight();
    }

    public string GetInteractPrompt()
    {
        string action = isOn ? "Turn Off" : "Turn On";
        return $"Press [E] to {action} {lightName}";
    }
    // ------------------------------------

    public void ToggleLight()
    {
        isOn = !isOn;
        UpdateLightState();
        Debug.Log($"Lantern toggled: {(isOn ? "ON" : "OFF")}");
    }

    private void UpdateLightState()
    {
        if (targetLight != null)
        {
            targetLight.enabled = isOn;
            targetLight.color = warmLightColor;
            targetLight.intensity = onIntensity;
        }

        // Handle Material Emission Glow Toggle
        if (lanternMeshRenderer != null && lanternMeshRenderer.materials.Length > materialIndex)
        {
            Material mat = lanternMeshRenderer.materials[materialIndex];
            if (isOn)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", warmLightColor * onIntensity);
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}