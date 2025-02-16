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

        [SerializeField] public SaveValue<bool> isTeleporterActive = new SaveValue<bool>("isTeleporterActive"); // To save the teleporter state

        [SerializeField] public SaveValue<bool> isEscapeTextActive = new SaveValue<bool>("isEscapeTextActive"); // Save EscapeText visibility
        [SerializeField] public SaveValue<bool> isMiniGameStarterEnabled = new SaveValue<bool>("isMiniGameStarterEnabled"); // Save MiniGameStarter collider state

        [SerializeField] public SaveValue<bool[]> lockStates = new SaveValue<bool[]>("lockStates", new bool[4]);
        [SerializeField] public SaveValue<bool[]> keyAnimationStates = new SaveValue<bool[]>("keyAnimationStates", new bool[4]);
    }
}
