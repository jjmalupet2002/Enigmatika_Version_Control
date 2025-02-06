using CarterGames.Assets.SaveManager;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "PortalRoomTrapLockSaveObject")]
    public class PortalRoomTrapLockSaveObject : SaveObject
    {
        [SerializeField] public SaveValue<Vector3> gatePosition = new SaveValue<Vector3>("gatePosition");
        [SerializeField] public SaveValue<bool> isGateUnlocked = new SaveValue<bool>("isGateUnlocked"); // To save the unlocked state
        [SerializeField] public SaveValue<int> currentGateState = new SaveValue<int>("currentGateState"); // To save the gate state (Closed = 0, Open = 1)

    }
}
