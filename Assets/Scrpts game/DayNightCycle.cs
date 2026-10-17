using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time & Light Settings")]
    [Tooltip("Duration of sunset-to-midnight cycle in real-time seconds")]
    public float cycleDurationInSeconds = 60f;
    
    [SerializeField] private Light directionalLight;

    [Header("Realistic Sunset (Soft Golden Hour)")]
    // Soft golden-amber sunset light instead of harsh neon orange
    public Color sunsetLightColor = new Color(1.0f, 0.72f, 0.48f); // Soft Golden Amber
    public Color sunsetSkyColor = new Color(0.82f, 0.52f, 0.38f);   // Subtle Sunset Sky
    public Color sunsetEquatorColor = new Color(0.65f, 0.45f, 0.4f);  // Soft Horizon Blend
    public Color sunsetGroundColor = new Color(0.4f, 0.28f, 0.2f);    // Warm Earth Ground (Keeps lanterns warm)

    [Header("Atmospheric Midnight")]
    // Deep moonlight navy blue
    public Color midnightLightColor = new Color(0.25f, 0.35f, 0.55f); // Cool Moonlight
    public Color midnightSkyColor = new Color(0.05f, 0.08f, 0.18f);   // Deep Night Sky
    public Color midnightEquatorColor = new Color(0.04f, 0.06f, 0.12f); // Night Horizon
    public Color midnightGroundColor = new Color(0.12f, 0.08f, 0.06f);  // Warm Dark Ground (Protects lantern shadows)

    [Header("Light Intensity")]
    public float sunsetIntensity = 1.0f;
    public float midnightIntensity = 0.2f;

    [Header("Rotation Angles")]
    // Low golden-hour horizon down to midnight angle
    public Vector3 sunsetRotation = new Vector3(20f, -45f, 0f);
    public Vector3 midnightRotation = new Vector3(75f, -45f, 0f);

    private float currentProgress = 0f;

    void Start()
    {
        if (directionalLight == null)
        {
            directionalLight = GetComponent<Light>();
        }

        // Enable Trilight ambient mode for separate Sky/Equator/Ground ambient control
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
    }

    void Update()
    {
        if (cycleDurationInSeconds > 0)
        {
            currentProgress += Time.deltaTime / cycleDurationInSeconds;
            if (currentProgress > 1.0f)
            {
                currentProgress = 1.0f; // Clamp at midnight
            }
        }

        ApplyLightingProgress(currentProgress);
    }

    private void ApplyLightingProgress(float t)
    {
        if (directionalLight != null)
        {
            directionalLight.transform.rotation = Quaternion.Euler(Vector3.Lerp(sunsetRotation, midnightRotation, t));
            directionalLight.color = Color.Lerp(sunsetLightColor, midnightLightColor, t);
            directionalLight.intensity = Mathf.Lerp(sunsetIntensity, midnightIntensity, t);
        }

        // Apply 3-zone ambient blend so ground shadows retain warm lantern glow
        RenderSettings.ambientSkyColor = Color.Lerp(sunsetSkyColor, midnightSkyColor, t);
        RenderSettings.ambientEquatorColor = Color.Lerp(sunsetEquatorColor, midnightEquatorColor, t);
        RenderSettings.ambientGroundColor = Color.Lerp(sunsetGroundColor, midnightGroundColor, t);
    }
}