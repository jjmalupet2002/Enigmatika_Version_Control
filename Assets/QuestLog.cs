using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLog : MonoBehaviour
{
    [Header("Quest System References")]
    public QuestManager questManager;
    public List<MainQuest> mainQuestList;

    [Header("Quest Buttons")]
    public Button quest1BTN;
    public Button quest2BTN;
    public Button quest3BTN;
    public Button quest4BTN;

    [Header("Page List (Logical Representation)")]
    public List<int> pageList = new List<int> { 0, 1, 2, 3 }; // Logical representation of pages

    [Header("Objective View Settings")]
    public TextMeshProUGUI mainObjectiveText;
    public TextMeshProUGUI activeCriteriaText;
    public GameObject scrollView;
    public TextMeshProUGUI notCompletedText; // Separate Not Completed TextMeshProUGUI

    [Header("Criteria Text List")]
    public List<TextMeshProUGUI> criteriaTextList;

    private int currentQuestIndex = -1;

    private void Start()
    {
        // Initialize button listeners
        quest1BTN.onClick.AddListener(() => DisplayQuest(0));
        quest2BTN.onClick.AddListener(() => DisplayQuest(1));
        quest3BTN.onClick.AddListener(() => DisplayQuest(2));
        quest4BTN.onClick.AddListener(() => DisplayQuest(3));

        // Initialize the scroll view and not completed text as disabled
        scrollView.SetActive(false);
        notCompletedText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Refresh the active quest display in real-time
        if (currentQuestIndex >= 0)
        {
            DisplayQuest(currentQuestIndex);
        }
    }

    private void DisplayQuest(int questIndex)
    {
        if (questIndex < 0 || questIndex >= mainQuestList.Count)
        {
            return;
        }

        currentQuestIndex = questIndex; // Track the currently displayed quest index
        MainQuest selectedQuest = mainQuestList[questIndex];

        // For each quest page, show "Not Started" text when the quest is not started
        notCompletedText.text = "Quest " + (questIndex + 1) + " Not Started";

        if (selectedQuest.status == QuestEnums.QuestStatus.NotStarted)
        {
            // Quest not started, disable the scroll view and show "Not completed" text
            scrollView.SetActive(false);
            notCompletedText.gameObject.SetActive(true);
            mainObjectiveText.text = "";
            activeCriteriaText.text = "";
            foreach (TextMeshProUGUI criteriaText in criteriaTextList)
            {
                criteriaText.text = "";
            }
        }
        else
        {
            // Quest is active or completed, display the quest information
            scrollView.SetActive(true);
            notCompletedText.gameObject.SetActive(false);
            mainObjectiveText.text = selectedQuest.questDescription;

            // Find the active criteria and display it
            QuestCriteria activeCriteria = selectedQuest.questCriteriaList.Find(criteria => criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress);
            if (activeCriteria != null)
            {
                activeCriteriaText.text = "In Progress: " + activeCriteria.criteriaName;
            }
            else
            {
                activeCriteriaText.text = "All criteria completed";
            }

            // Display all completed criteria
            int criteriaIndex = 0;
            foreach (QuestCriteria criteria in selectedQuest.questCriteriaList)
            {
                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed && criteriaIndex < criteriaTextList.Count)
                {
                    criteriaTextList[criteriaIndex].text = criteria.criteriaName;
                    criteriaIndex++;
                }
            }

            // Clear remaining criteria text slots
            for (int i = criteriaIndex; i < criteriaTextList.Count; i++)
            {
                criteriaTextList[i].text = "";
            }
        }
    }

    public void UpdateQuestLog()
    {
        // Method to be called when a quest criteria is updated
        for (int i = 0; i < mainQuestList.Count; i++)
        {
            MainQuest quest = mainQuestList[i];
            bool criteriaUpdated = false;

            foreach (QuestCriteria criteria in quest.questCriteriaList)
            {
                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed)
                {
                    criteriaUpdated = true;
                }
            }

            if (criteriaUpdated && i == currentQuestIndex)
            {
                DisplayQuest(i);
            }
        }
    }
}