using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to represent item data in the inventory
[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public bool isClueItem;
    public bool isGeneralItem;
    public bool isUsable;
    public bool isUsingItem;
    public string keyId;
    public bool isNote;
    public GameObject noteUI;
    public bool hasBeenInspected; // New flag

    public ItemData(string name, Sprite icon, string description, bool clueItem, bool generalItem, bool usable, bool usingItem, bool note, bool inspected, GameObject noteUI = null, string keyId = "")
    {
        itemName = name;
        itemIcon = icon;
        itemDescription = description;
        isClueItem = clueItem;
        isGeneralItem = generalItem;
        isUsable = usable;
        isUsingItem = usingItem;
        this.keyId = keyId;
        isNote = note;
        hasBeenInspected = inspected; // Initialize from constructor
        this.noteUI = noteUI;
    }
}

