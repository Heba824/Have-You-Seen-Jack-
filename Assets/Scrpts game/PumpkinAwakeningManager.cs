using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PumpkinAwakeningManager : MonoBehaviour, IInteractable
{
    [Header("Cinemachine Cutscene Camera")]
    [SerializeField] private CinemachineCamera cutsceneVirtualCamera;

    [Header("Potion Visual Effects")]
    [SerializeField] private GameObject bubblingEffectPrefab;
    [SerializeField] private Transform potSpawnPoint;

    [Header("Pumpkin Setup")]
    [SerializeField] private GameObject pumpkin;
    [SerializeField] private Transform landingZoneTarget;

    [Header("Jump Parameters")]
    public float verticalJumpForce = 5f;
    public float forwardJumpForce = 3f;

    [Header("Area Gate")]
    [SerializeField] private GameObject areaGate;

    [Header("Generated Potion Effect (fallback only)")]
    [SerializeField] private bool useGeneratedEffect = true;
    [SerializeField] private Color potionColor = new Color(0.2f, 1f, 0.35f, 1f);
    [SerializeField] private float effectLifetime = 4f;

    private Rigidbody pumpkinRigidbody;
    private bool hasTriggeredAwakening;

    private void Start()
    {
        if (cutsceneVirtualCamera != null)
            cutsceneVirtualCamera.Priority.Value = 0;

        if (pumpkin != null)
        {
            pumpkinRigidbody = pumpkin.GetComponent<Rigidbody>();

            if (pumpkinRigidbody != null)
                pumpkinRigidbody.isKinematic = true;
        }

        if (areaGate != null)
            areaGate.SetActive(true);
    }

    public void Interact()
    {
        if (hasTriggeredAwakening)
            return;

        hasTriggeredAwakening = true;
        StartCoroutine(PlayPotionCutsceneRoutine());
    }

    public string GetInteractPrompt()
    {
        return hasTriggeredAwakening ? "" : "Press [E] to Brew Potion";
    }

    private IEnumerator PlayPotionCutsceneRoutine()
    {
        Debug.Log("Cutscene Started!");

        if (cutsceneVirtualCamera != null)
            cutsceneVirtualCamera.Priority.Value = 20;

        yield return new WaitForSeconds(0.5f);

        PlayPotionEffect();

        yield return new WaitForSeconds(1.2f);

        if (pumpkinRigidbody != null)
        {
            pumpkinRigidbody.isKinematic = false;

            Vector3 horizontalDirection = pumpkin.transform.forward;

            if (landingZoneTarget != null)
            {
                horizontalDirection =
                    landingZoneTarget.position - pumpkin.transform.position;
                horizontalDirection.y = 0f;

                if (horizontalDirection.sqrMagnitude > 0.001f)
                    horizontalDirection.Normalize();
                else
                    horizontalDirection = pumpkin.transform.forward;
            }

            Vector3 jumpDirection =
                horizontalDirection * forwardJumpForce +
                Vector3.up * verticalJumpForce;

            pumpkinRigidbody.AddForce(jumpDirection, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1.5f);

        if (areaGate != null)
            areaGate.SetActive(false);

        if (cutsceneVirtualCamera != null)
            cutsceneVirtualCamera.Priority.Value = 0;

        Debug.Log("Cutscene Finished!");
    }

    private void PlayPotionEffect()
    {
        if (potSpawnPoint == null)
        {
            Debug.LogError("Potion effect: assign Pot Spawn Point in the Inspector.");
            return;
        }

        // Your assigned VFX takes priority
        if (bubblingEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                bubblingEffectPrefab,
                potSpawnPoint.position,
                potSpawnPoint.rotation);

            effect.SetActive(true);

            // Make sure every particle system in the prefab actually plays
            foreach (var ps in effect.GetComponentsInChildren<ParticleSystem>(true))
            {
                ps.gameObject.SetActive(true);
                ps.Clear(true);
                ps.Play(true);
            }

            Debug.Log("Prefab VFX spawned at " + effect.transform.position);
            Destroy(effect, effectLifetime);
        }
        else if (useGeneratedEffect)
        {
            CreateGeneratedPotionEffect(); // fallback only
        }
        else
        {
            Debug.LogError("No potion effect prefab is assigned.");
        }
    }

    private void CreateGeneratedPotionEffect()
    {
        GameObject effectRoot = new GameObject("GeneratedPotionVFX");
        effectRoot.transform.position =
            potSpawnPoint.position + Vector3.up * 0.35f;

        GameObject particlesObject = new GameObject("PotionParticles");
        particlesObject.transform.SetParent(effectRoot.transform, false);
        particlesObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        ParticleSystem particles = particlesObject.AddComponent<ParticleSystem>();
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = particles.main;
        main.playOnAwake = false;
        main.duration = effectLifetime;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.28f);
        main.startColor = potionColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 150;

        var emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 35),
            new ParticleSystem.Burst(0.35f, 20),
            new ParticleSystem.Burst(0.7f, 20)
        });

        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.35f;
        shape.arc = 360f;

        // x, y, z must all use the same curve mode
        var velocity = particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.4f, 1.4f);
        velocity.z = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(potionColor, 0f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(potionColor, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = gradient;

        var renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");

        if (shader != null)
        {
            Material material = new Material(shader);
            material.color = potionColor;
            renderer.material = material;
        }
        else
        {
            Debug.LogWarning("No compatible particle shader found.");
        }

        particles.Play();
        Destroy(effectRoot, effectLifetime + 1f);
    }
}