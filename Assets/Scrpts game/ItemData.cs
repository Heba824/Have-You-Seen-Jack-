using UnityEngine;

public enum PotionType
{
    None,
    Camouflage,
    GhostWalk,
    PulpShockwave
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName = "Candy Bar";
    public Sprite icon;
    public GameObject itemPrefab; // 3D model if dropped or spawned
    public Sprite itemIcon;
    [Header("Potion Properties")]
    public PotionType potionType = PotionType.None; // Set to None for Candy Bars or regular items
}