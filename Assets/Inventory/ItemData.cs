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
    public bool isUsingItem;                 // Indicates if the item is currently in use
    public readonly string keyId;            // The ID of the key (if this is a key item)
    public readonly bool isNote;             // Is this item a note?
    public GameObject noteUI;                // UI for displaying the note

    // Constructor for easy instantiation
    public ItemData(string name, Sprite icon, string description, bool clueItem, bool generalItem, bool usable, bool usingItem, bool note, GameObject noteUI = null, string keyId = "")
    {
        itemName = name;
        itemIcon = icon;
        itemDescription = description;
        isClueItem = clueItem;
        isGeneralItem = generalItem;
        isUsable = usable;
        isUsingItem = usingItem; // Initialize with passed value
        this.keyId = keyId;      // Initialize keyId (empty string for non-key items)
        isNote = note;
        this.noteUI = noteUI; // Initialize note UI
    }
}
