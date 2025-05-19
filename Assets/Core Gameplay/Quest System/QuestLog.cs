using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestLog : MonoBehaviour
{
    [Header("Quest System References")]
    public QuestManager questManager;
    public List<MainQuest> mainQuestList;

    [Header("Objective View Settings")]
    public TextMeshProUGUI QuestNameText;
    public TextMeshProUGUI mainObjectiveText;
    public TextMeshProUGUI activeCriteriaText;
    public GameObject scrollView;
    public TextMeshProUGUI notCompletedText;

    [Header("Criteria Text List")]
    public List<TextMeshProUGUI> criteriaTextList;

    [Header("Checkmark List")]
    public List<GameObject> checkmarkList;  // List to reference the 30 checkmark GameObjects

    private int currentQuestIndex = -1;

    private void Start()
    {
        scrollView.SetActive(false);
        notCompletedText.gameObject.SetActive(false);
        DisplayActiveQuest();
    }

    private void Update()
    {
        DisplayActiveQuest();
    }

    private void DisplayActiveQuest()
    {
        MainQuest activeQuest = mainQuestList.Find(q => q.status == QuestEnums.QuestStatus.InProgress);

        if (activeQuest == null)
        {
            currentQuestIndex = -1;
            scrollView.SetActive(false);
            notCompletedText.gameObject.SetActive(true);
            notCompletedText.text = "Quest Not Started";

            QuestNameText.text = "";
            mainObjectiveText.text = "";
            activeCriteriaText.text = "";

            foreach (TextMeshProUGUI criteriaText in criteriaTextList)
            {
                criteriaText.text = "";
            }

            // Disable all checkmarks if no active quest
            foreach (GameObject checkmark in checkmarkList)
            {
                checkmark.SetActive(false);
            }

            return;
        }

        currentQuestIndex = mainQuestList.IndexOf(activeQuest);

        // Display quest info
        scrollView.SetActive(true);
        notCompletedText.gameObject.SetActive(false);

        QuestNameText.text = activeQuest.questName;
        mainObjectiveText.text = activeQuest.questDescription;

        QuestCriteria activeCriteria = activeQuest.questCriteriaList.Find(c => c.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress);
        activeCriteriaText.text = activeCriteria != null
            ? "In Progress: " + activeCriteria.criteriaName
            : "All criteria completed";

        // Show all criteria with status and update checkmark list
        for (int i = 0; i < criteriaTextList.Count; i++)
        {
            if (i < activeQuest.questCriteriaList.Count)
            {
                QuestCriteria criteria = activeQuest.questCriteriaList[i];

                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed)
                {
                    criteriaTextList[i].text = criteria.criteriaName;

                    // Enable corresponding checkmark GameObject
                    if (i < checkmarkList.Count)
                    {
                        checkmarkList[i].SetActive(true);
                    }
                }
                else
                {
                    criteriaTextList[i].text = criteria.criteriaName;

                    // Disable corresponding checkmark GameObject
                    if (i < checkmarkList.Count)
                    {
                        checkmarkList[i].SetActive(false);
                    }
                }
            }
            else
            {
                criteriaTextList[i].text = "";
                // Ensure no checkmarks are active if there are fewer criteria than available slots
                if (i < checkmarkList.Count)
                {
                    checkmarkList[i].SetActive(false);
                }
            }
        }
    }

    public void UpdateQuestLog()
    {
        if (currentQuestIndex >= 0 && currentQuestIndex < mainQuestList.Count)
        {
            MainQuest quest = mainQuestList[currentQuestIndex];
            bool criteriaUpdated = false;

            foreach (QuestCriteria criteria in quest.questCriteriaList)
            {
                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed)
                {
                    criteriaUpdated = true;
                }
            }

            if (criteriaUpdated)
            {
                DisplayActiveQuest();
            }
        }
    }
}
