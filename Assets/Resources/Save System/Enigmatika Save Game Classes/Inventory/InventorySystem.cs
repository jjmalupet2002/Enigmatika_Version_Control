using System.Collections.Generic;
using CarterGames.Assets.SaveManager;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "InventorySystem")]
    public class InventorySystem : SaveObject
    {
        public List<string> itemNames = new List<string>(); // Store item names
        public List<Sprite> itemIcons = new List<Sprite>(); // Store item icons
        public List<string> itemDescriptions = new List<string>(); // Store item descriptions
        public List<bool> isClueItems = new List<bool>(); // Store clue item flags
        public List<bool> isGeneralItems = new List<bool>(); // Store general item flags
        public List<bool> isUsableItems = new List<bool>(); // Store usability flags
        public List<bool> isUsingItems = new List<bool>(); // Store item usage status
        public List<string> keyIds = new List<string>(); // Store key IDs
        public List<bool> isNotes = new List<bool>(); // Store note flags
        public List<GameObject> noteUIs = new List<GameObject>(); // Store note UI objects
        public List<bool> itemInspectionStatus = new List<bool>(); // Store inspection status
    }
}
