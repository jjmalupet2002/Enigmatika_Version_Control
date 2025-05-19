using CarterGames.Assets.SaveManager;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "DoorSaveObject")]
    public class DoorSaveObjectSaveObject : SaveObject
    {
        [SerializeField] public SaveValue<bool[]> lockStates = new SaveValue<bool[]>("lockStates"); // To save lock/unlock states
        [SerializeField] public SaveValue<bool[]> openStates = new SaveValue<bool[]>("openStates"); // To save open/closed states

    }
}
