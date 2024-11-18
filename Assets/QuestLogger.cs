using System.Collections.Generic;
using UnityEngine;

public class QuestLogger : MonoBehaviour
{
    public List<MainQuest> mainQuests;  // List of all available main quests
    private MainQuest currentQuest;  // The currently selected quest
    public QuestManager questManager;  // Reference to QuestManager to call AcceptQuest

    void Update()
    {
        // Check if the N key is pressed to cycle through available quests
        if (Input.GetKeyDown(KeyCode.N))
        {
            HoverNextQuest();
        }

        // Check if the S key is pressed to start the current selected quest
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartSelectedQuest();
        }

        // Check if the L key is pressed to log active criteria for the current quest
        if (Input.GetKeyDown(KeyCode.L))
        {
            LogActiveCriteriaForCurrentQuest();
        }
    }

    // Cycle through available quests
    private void HoverNextQuest()
    {
        if (mainQuests.Count == 0)
        {
            UnityEngine.Debug.LogWarning("No available quests to hover through.");
            return;
        }

        // If no quest is currently selected, choose the first one
        if (currentQuest == null)
        {
            currentQuest = mainQuests[0];
        }
        else
        {
            // Find the next available quest (not in progress)
            int currentIndex = mainQuests.IndexOf(currentQuest);
            int nextIndex = (currentIndex + 1) % mainQuests.Count;
            currentQuest = mainQuests[nextIndex];
        }

        UnityEngine.Debug.Log($"Hovering over quest: {currentQuest.questName}");
    }

    // Start the current selected quest
    private void StartSelectedQuest()
    {
        if (currentQuest == null)
        {
            UnityEngine.Debug.LogWarning("No quest is selected to start.");
            return;
        }

        if (currentQuest.status == QuestEnums.QuestStatus.NotStarted)
        {
            // Call AcceptQuest from QuestManager here
            questManager.AcceptQuest(currentQuest);  // This triggers the 'Initializing Quest...' log
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Quest {currentQuest.questName} is already started or completed.");
        }
    }

    // Log the active criteria for the current selected quest
    private void LogActiveCriteriaForCurrentQuest()
    {
        if (currentQuest != null)
        {
            currentQuest.LogActiveCriteria();
        }
        else
        {
            UnityEngine.Debug.LogWarning("No quest selected to log criteria.");
        }
    }
}