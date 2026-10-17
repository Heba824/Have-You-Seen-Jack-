using UnityEngine;
using System.Collections;

public class PulpShockwaveEffect : MonoBehaviour
{
    [Header("Visual Effects")]
    [Tooltip("Assign your custom Shockwave VFX Prefab here in the Inspector")]
    [SerializeField] private GameObject shockwaveVFXPrefab;

    [Header("Shockwave Settings")]
    [SerializeField] private float effectDuration = 10.0f; // How long the power lasts (seconds)
    [SerializeField] private float shockwaveRadius = 5.0f;
    [SerializeField] private float shockwaveForce = 12.0f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isShockwaveActive = false;
    private float timeRemaining = 0f;

    // Matches the method call in PlayerInventory.cs
    public void TriggerShockwave()
    {
        isShockwaveActive = true;
        timeRemaining = effectDuration;
        Debug.Log("Pulp Shockwave Active! Press SPACEBAR to smash enemies!");
    }

    void Update()
    {
        if (!isShockwaveActive) return;

        // Countdown timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            DeactivateShockwave();
            return;
        }

        // Press SPACEBAR to trigger attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteShockwaveAttack();
        }
    }

    private void ExecuteShockwaveAttack()
    {
        Debug.Log("BOOM! Pulp Shockwave Slam!");

        // 1. INSTANTIATE YOUR ASSIGNED VFX PREFAB
        if (shockwaveVFXPrefab != null)
        {
            GameObject vfxInstance = Instantiate(shockwaveVFXPrefab, transform.position, transform.rotation);
            
            // Ensure particle system plays if attached
            ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            Destroy(vfxInstance, 3.0f); // Clean up after effect finishes
        }

        // 2. PHYSICS: Detect and knock back enemies
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, shockwaveRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (enemy.transform.position - transform.position).normalized + Vector3.up * 0.5f;
                rb.AddForce(pushDir * shockwaveForce, ForceMode.Impulse);
            }
        }
    }

    private void DeactivateShockwave()
    {
        isShockwaveActive = false;
        Debug.Log("Shockwave effect wore off. Back to normal.");
    }

    private void OnDrawGizmosSelected()
    {
        if (isShockwaveActive)
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
        }
    }
}