using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    public Transform handHoldPoint; // Assign your RightHand / ItemHold bone here in Inspector
    private Animator animator;

    [Header("Potion Effect References")]
    [SerializeField] private PumpkinCamouflageEffect camoEffect;
    [SerializeField] private GhostPotionEffect ghostEffect;
    [SerializeField] private PulpShockwaveEffect shockwaveEffect;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Auto-fetch potion components if attached to player
        if (camoEffect == null) camoEffect = GetComponent<PumpkinCamouflageEffect>();
        if (ghostEffect == null) ghostEffect = GetComponent<GhostPotionEffect>();
        if (shockwaveEffect == null) shockwaveEffect = GetComponent<PulpShockwaveEffect>();
    }

    void Update()
    {
        // Press 'Q' to drink/use the first potion available in inventory
        if (Input.GetKeyDown(KeyCode.Q) && items.Count > 0)
        {
            // Find the first item that is a potion
            ItemData potionToDrink = items.Find(i => i.potionType != PotionType.None);

            if (potionToDrink != null)
            {
                Debug.Log($"Drinking potion: {potionToDrink.itemName}");
                UseItem(potionToDrink); // This triggers your switch-case power effect and removes it
            }
            else
            {
                // Fallback if it's a regular item
                UseItem(items[0]);
            }
        }
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);

        if (animator != null)
        {
            animator.SetTrigger("Gather");
        }

        Debug.Log("Picked up: " + item.itemName);
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public bool RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            return true;
        }
        return false;
    }

    public void UseItem(ItemData item)
    {
        if (!items.Contains(item)) return;

        // Trigger the corresponding potion effect
        switch (item.potionType)
        {
            case PotionType.Camouflage:
                if (camoEffect != null) camoEffect.ActivateCamouflage();
                break;
            case PotionType.GhostWalk:
                if (ghostEffect != null) ghostEffect.ActivateGhostMode();
                break;
            case PotionType.PulpShockwave:
                if (shockwaveEffect != null) shockwaveEffect.TriggerShockwave();
                break;
            case PotionType.None:
                Debug.Log($"Ate/Used regular item: {item.itemName}");
                break;
        }

        // Consume and remove from inventory
        RemoveItem(item);
    }
}