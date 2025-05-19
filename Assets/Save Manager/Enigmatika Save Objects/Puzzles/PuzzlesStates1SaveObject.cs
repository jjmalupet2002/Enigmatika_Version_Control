using CarterGames.Assets.SaveManager;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "PuzzlesStates1SaveObject", menuName = "SaveObjects/PuzzlesStates1SaveObject")]
    public class PuzzlesStates1SaveObject : SaveObject
    {
        [SerializeField] public SaveValue<Vector3> spikePosition = new SaveValue<Vector3>("spikePosition");
        [SerializeField] public SaveValue<bool> isSpikeUnlocked = new SaveValue<bool>("isSpikeUnlocked");
        [SerializeField] public SaveValue<int> currentSpikeState = new SaveValue<int>("currentSpikeState"); // 0 = Closed, 1 = Open

        // Added safe state values
        [SerializeField] public SaveValue<bool> isSafeOpened = new SaveValue<bool>("isSafeOpened");

        //Added Knight Statue and Thief Hideout Entrance states
        [SerializeField] public SaveValue<Vector3> knight1Rotation = new SaveValue<Vector3>("knight1Rotation");
        [SerializeField] public SaveValue<Vector3> knight2Rotation = new SaveValue<Vector3>("knight2Rotation");
        [SerializeField] public SaveValue<bool> allStatueRotated = new SaveValue<bool>("allStatueRotated");
        [SerializeField] public SaveValue<Vector3> thiefHideoutPosition = new SaveValue<Vector3>("thiefHideoutPosition");
    }
}
