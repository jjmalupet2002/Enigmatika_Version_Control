using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class CaseFileDisplay : MonoBehaviour
{
    [Header("Quest System References")]
    public QuestManager questManager;
    public List<MainQuest> mainQuestList;

    [Header("Case File Display Settings")]
    public Text caseFileNameText;
    public Text readingSkillText;
    public Text readingQuestionText;
    public Text fileNumberText;

    [Header("Tracked Clue Slots")]
    public Image clueSlot1;
    public Image clueSlot2;
    public Image clueSlot3;
    public Image clueSlot4;
    private List<Image> trackedClueSlots;

    [Header("Check Marker List")]
    public Image checkmark1;
    public Image checkmark2;
    public Image checkmark3;
    public Image checkmark4;

    [Header("Clue Tracker List")]
    public List<Clue> clueTrackerList;

    [Header("UI Feedback")]
    public GameObject noCluesCollected;  // <-- NEW

    private int currentQuestIndex = -1;
    private int clueIndex = 0;
    
    private void Start()
    {
        trackedClueSlots = new List<Image> { clueSlot1, clueSlot2, clueSlot3, clueSlot4 };
        DisplayActiveQuest();

        // Initial state
        if (noCluesCollected != null)
            noCluesCollected.SetActive(true);
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
            caseFileNameText.text = "No active quest started";
            readingSkillText.text = "No active quest started";
            readingQuestionText.text = "No active quest started";
            fileNumberText.text = "Case File: N/A";
            return;
        }

        currentQuestIndex = mainQuestList.IndexOf(activeQuest);
        caseFileNameText.text = "Quest: " + activeQuest.questName;
        readingSkillText.text = activeQuest.readingSkill;
        readingQuestionText.text = activeQuest.readingQuestion;
        fileNumberText.text = "Case File: " + "00" + (currentQuestIndex + 1);
    }

    public void ClueSlot1Complete()
    {
        FinalizeClueSlot(clueSlot1, checkmark1);
    }

    public void ClueSlot2Complete()
    {
        FinalizeClueSlot(clueSlot2, checkmark2);
    }

    public void ClueSlot3Complete()
    {
        FinalizeClueSlot(clueSlot3, checkmark3);
    }

    public void ClueSlot4Complete()
    {
        FinalizeClueSlot(clueSlot4, checkmark4);
    }

    public void TriggerClue(int clueIndex)
    {
        if (clueIndex < clueTrackerList.Count)
        {
            var clue = clueTrackerList[clueIndex];

            if (clue != null)
            {
                Image targetSlot = clueIndex < trackedClueSlots.Count ? trackedClueSlots[clueIndex] : null;

                if (targetSlot != null)
                {
                    targetSlot.sprite = clue.clueSprite;
                    targetSlot.gameObject.SetActive(true);
                    targetSlot.GetComponent<Button>().onClick.AddListener(() => ShowLearningModule(clue));

                    clue.wasTriggered = true;

                    UnityEngine.Debug.Log($"Clue {clueIndex + 1} triggered: {clue.clueContext}");

                    if (noCluesCollected != null)
                        noCluesCollected.SetActive(false); // Hide the warning
                }
            }
        }
    }

    public void TriggerNextClue()
    {
        for (int i = 0; i < clueTrackerList.Count; i++)
        {
            if (!clueTrackerList[i].wasTriggered)
            {
                for (int j = 0; j < trackedClueSlots.Count; j++)
                {
                    if (!trackedClueSlots[j].gameObject.activeSelf)
                    {
                        trackedClueSlots[j].sprite = clueTrackerList[i].clueSprite;
                        trackedClueSlots[j].gameObject.SetActive(true);

                        int capturedIndex = i;
                        trackedClueSlots[j].GetComponent<Button>().onClick.AddListener(() =>
                        {
                            clueTrackerList[capturedIndex].learningModule.SetActive(true);
                        });

                        clueTrackerList[i].wasTriggered = true;

                        UnityEngine.Debug.Log($"Next clue triggered: {clueTrackerList[i].clueContext}");

                        if (noCluesCollected != null)
                            noCluesCollected.SetActive(false); // Hide the warning

                        return;
                    }
                }
            }
        }
    }

    private void FinalizeClueSlot(Image clueSlot, Image checkmark)
    {
        if (clueSlot != null)
        {
            // Disable button
            Button button = clueSlot.GetComponent<Button>();
            if (button != null)
                button.interactable = false;

            // Darken and lower opacity
            Color originalColor = clueSlot.color;
            clueSlot.color = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f, 0.5f);

            // Enable checkmark
            if (checkmark != null)
                checkmark.gameObject.SetActive(true);

            UnityEngine.Debug.Log($"{clueSlot.name} finalized: dimmed, disabled, and checkmarked.");
        }
    }

    private void ShowLearningModule(Clue clue)
    {
        if (clue.learningModule != null)
        {
            clue.learningModule.SetActive(true);
        }
    }
}

[System.Serializable]
public class Clue
{
    public Sprite clueSprite;
    public string clueContext;
    public GameObject learningModule;
    public bool wasTriggered = false;
}
