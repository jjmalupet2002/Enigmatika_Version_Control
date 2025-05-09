using UnityEngine;
using UnityEngine.UI;

public class CriteriaContextController : MonoBehaviour
{
    public Text tipTextUI;                     // Tip display UI
    public QuestManager questManager;         // Reference to QuestManager in Inspector

    void Update()
    {
        DisplayInProgressCriteria();
    }

    private void DisplayInProgressCriteria()
    {
        if (questManager == null) return;

        var activeQuests = questManager.GetActiveQuests();

        foreach (var questPair in activeQuests)
        {
            var quest = questPair.Value;

            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                {
                    UnityEngine.Debug.Log("Displaying InProgress Criteria Context: " + criteria.criteriaContext);
                    tipTextUI.text = criteria.criteriaContext;
                    return; // Show only one in-progress criteria at a time
                }
            }
        }

        tipTextUI.text = ""; // Clear if nothing is in progress
    }
}
