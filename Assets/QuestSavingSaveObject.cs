using CarterGames.Assets.SaveManager;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "QuestSavingSaveObject")]
    public class QuestSavingSaveObject : SaveObject
    {
        // To save the active quest names
        [SerializeField] public SaveValue<List<string>> activeQuestNames = new SaveValue<List<string>>("activeQuestNames");

        // To save the quest data
        [SerializeField] public SaveValue<List<QuestData>> questDataList = new SaveValue<List<QuestData>>("questDataList");
    }

    // Custom class to hold all the quest-related data in one place
    [System.Serializable]
    public class QuestData
    {
        public string questName;
        public QuestEnums.QuestStatus questStatus;
        public List<QuestCriteriaData> criteriaStatuses;
        public List<bool> criteriaCompletionStatus;
        public List<QuestObjectStateData> questObjectStates;
    }

    [System.Serializable]
    public class QuestCriteriaData
    {
        public string criteriaName;
        public QuestEnums.QuestCriteriaStatus criteriaStatus;
    }

    [System.Serializable]
    public class QuestObjectStateData
    {
        public string criteriaName;
        public bool isActive;
    }
}
