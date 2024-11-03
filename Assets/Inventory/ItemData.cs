using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to represent item data in the inventory
[System.Serializable]
public class ItemData
{
    // Item attributes
    public readonly string itemName;         // Name of the item
    public readonly Sprite itemIcon;         // Icon for the item
    public readonly string itemDescription;   // Description of the item
    public readonly bool isClueItem;         // Is this item a clue?
    public readonly bool isGeneralItem;      // Is this item a general item?
    public readonly bool isUsable;           // Is this item usable?

    // Constructor for easy instantiation
    public ItemData(string name, Sprite icon, string description, bool clueItem, bool generalItem, bool usable)
    {
        itemName = name;
        itemIcon = icon;
        itemDescription = description;
        isClueItem = clueItem;
        isGeneralItem = generalItem;
        isUsable = usable;
    }
}
