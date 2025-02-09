using CarterGames.Assets.SaveManager;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "HintPointsSaveObject", menuName = "ScriptableObjects/HintPointsSaveObject", order = 1)]
    public class HintPointsSaveObject : SaveObject
    {
        [SerializeField] public SaveValue<int> hintPoints = new SaveValue<int>("hintPoints"); // Save value for hint points
    }
}
