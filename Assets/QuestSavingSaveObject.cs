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

        // To save the quest statuses
        [SerializeField] public SaveValue<List<QuestStatusData>> questStatuses = new SaveValue<List<QuestStatusData>>("questStatuses");

        // To save the criteria statuses
        [SerializeField] public SaveValue<List<QuestCriteriaData>> criteriaStatuses = new SaveValue<List<QuestCriteriaData>>("criteriaStatuses");

        // To save the quest's criteria completion status
        [SerializeField] public SaveValue<List<CriteriaCompletionData>> criteriaCompletionStatus = new SaveValue<List<CriteriaCompletionData>>("criteriaCompletionStatus");
    }

    // Custom classes to hold the quest status, criteria status, and completion status
    [System.Serializable]
    public class QuestStatusData
    {
        public string questName;
        public QuestEnums.QuestStatus questStatus;
    }

    [System.Serializable]
    public class QuestCriteriaData
    {
        public string questName;
        public string criteriaName;
        public QuestEnums.QuestCriteriaStatus criteriaStatus;
    }

    [System.Serializable]
    public class CriteriaCompletionData
    {
        public string questName;
        public List<bool> completionStatus;
    }
}
