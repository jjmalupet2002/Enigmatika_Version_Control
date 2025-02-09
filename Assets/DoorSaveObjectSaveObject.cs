using CarterGames.Assets.SaveManager;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "DoorSaveObject")]
    public class DoorSaveObjectSaveObject : SaveObject
    {
        [SerializeField] public SaveValue<string> doorID = new SaveValue<string>("doorID");
        [SerializeField] public SaveValue<bool> isDoorLocked = new SaveValue<bool>("isDoorLocked");
        [SerializeField] public SaveValue<bool> isDoorOpened = new SaveValue<bool>("isDoorOpened");
        [SerializeField] public SaveValue<bool> isButtonPressed = new SaveValue<bool>("isButtonPressed");
        [SerializeField] public SaveValue<bool> isLeverUsed = new SaveValue<bool>("isLeverUsed");

    }
}
