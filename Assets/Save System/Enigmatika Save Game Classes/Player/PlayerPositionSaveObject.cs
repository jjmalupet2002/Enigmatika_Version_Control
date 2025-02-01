using CarterGames.Assets.SaveManager;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "PlayerPositionSaveObject", menuName = "Save/Player Position")]
    public class PlayerPositionSaveObject : SaveObject
    {
        // The variable that will hold the player's position
        public SaveValue<Vector3> playerPosition = new SaveValue<Vector3>("player_position");
    }
}
