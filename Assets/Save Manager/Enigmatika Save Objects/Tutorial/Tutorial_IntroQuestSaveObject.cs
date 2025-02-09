using CarterGames.Assets.SaveManager;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "Tutorial_IntroQuestSaveObject")]
    public class Tutorial_IntroQuestSaveObject : SaveObject
    {
        [SerializeField] public SaveValue<bool> isTutorialFinished = new SaveValue<bool>("isTutorialFinished");

        // New save values for quest state
        [SerializeField] public SaveValue<int> currentQuestState = new SaveValue<int>("currentQuestState");
    }
}
