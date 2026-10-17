using UnityEngine;

public class PotionDebuger : MonoBehaviour
{
    [SerializeField] private PumpkinCamouflageEffect camoScript;

    private void Update()
    {
        // Press 2 to activate Pumpkin Camouflage
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (camoScript != null)
            {
                camoScript.ActivateCamouflage();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
{
    GetComponent<GhostPotionEffect>().ActivateGhostMode();
}
    if (Input.GetKeyDown(KeyCode.Alpha3))
{
    GetComponent<PulpShockwaveEffect>().TriggerShockwave();
}
    }
}