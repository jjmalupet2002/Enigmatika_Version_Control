using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestAcceptUI : MonoBehaviour
{
    // Header 1: UI references
    public Button viewQuestButton;
    public GameObject questAcceptBackground;
    public Button exitButton;
    public Button startQuestButton; // Initially AcceptButton

    // Header 2: Button references
    public Text questNameText;
    public Text questDescriptionText; // Updated reference for quest description
    public Image rewardIconBackground;
    public Image questIconImage; // New reference for the quest icon
    public Button arrowLeftButton;
    public Button arrowRightButton;
    public Button turnInItemButton; // Replaces accept button after a quest has been started

    public List<MainQuest> mainQuests;  // List of all available main quests
    public List<Page> pages;  // List of pages, each containing quest UI elements
    public List<Sprite> questIconImages; // List of quest icon images
    public List<Image> rewardIconImages; // List of Image components for displaying multiple reward icons
    private int currentPageIndex = 0;  // To keep track of the current page index
    private MainQuest currentQuest;  // The currently selected quest
    public QuestManager questManager;  // Reference to QuestManager to call AcceptQuest

    void Start()
    {
        // Initialize UI elements
        questAcceptBackground.SetActive(false);
        turnInItemButton.gameObject.SetActive(false);
        startQuestButton.onClick.AddListener(StartSelectedQuest);
        exitButton.onClick.AddListener(CloseQuestUI);
        viewQuestButton.onClick.AddListener(OpenQuestUI);

        arrowLeftButton.onClick.AddListener(PreviousPage);
        arrowRightButton.onClick.AddListener(NextPage);

        // Initialize pages with quests
        InitializePages();
    }

    void InitializePages()
    {
        if (mainQuests.Count == 0 || pages.Count == 0)
        {
            UnityEngine.Debug.LogWarning("No available quests or pages to display.");
            return;
        }

        for (int i = 0; i < pages.Count; i++)
        {
            if (i < mainQuests.Count)
            {
                pages[i].quest = mainQuests[i];
                pages[i].questIcon = questIconImages[i % questIconImages.Count]; // Assign quest icon image based on index
                // Add event listeners for hover events
                int index = i;  // Local copy of the index for the lambda
                pages[i].questIconImage = questIconImage; // Assign the Image component for the quest icon
                pages[i].questIconImage.GetComponent<Button>().onClick.AddListener(() => OnQuestIconHover(index));
            }
            else
            {
                pages[i].quest = null;  // No quest for this page
            }
        }

        UpdateQuestUI();
    }

    void OpenQuestUI()
    {
        questAcceptBackground.SetActive(true);
        currentPageIndex = 0;  // Reset to ensure it starts from the first page
        UpdateQuestUI();
    }

    void CloseQuestUI()
    {
        questAcceptBackground.SetActive(false);
    }

    void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdateQuestUI();
        }
    }

    void NextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            UpdateQuestUI();
        }
    }

    void UpdateQuestUI()
    {
        if (pages.Count == 0 || currentPageIndex == -1)
        {
            UnityEngine.Debug.LogWarning("No available pages to display.");
            return;
        }

        Page currentPage = pages[currentPageIndex];
        currentQuest = currentPage.quest;

        if (currentQuest != null)
        {
            questNameText.text = currentQuest.questName;
            questDescriptionText.text = currentQuest.questDescription;  // Update to use the description from MainQuest
            rewardIconBackground.sprite = currentPage.rewardIcon;  // Update the reward icon background
            questIconImage.sprite = currentPage.questIcon;  // Update the quest icon

            // Update the reward icons based on the current page's reward icon list
            for (int i = 0; i < rewardIconImages.Count; i++)
            {
                if (i < currentPage.rewardIcons.Count)
                {
                    rewardIconImages[i].sprite = currentPage.rewardIcons[i];
                    rewardIconImages[i].gameObject.SetActive(true);
                }
                else
                {
                    rewardIconImages[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            questNameText.text = "No Quest";
            questDescriptionText.text = "No description available.";
            rewardIconBackground.sprite = null;  // No reward icon
            questIconImage.sprite = null;  // No quest icon

            // Hide all reward icons
            foreach (var rewardIconImage in rewardIconImages)
            {
                rewardIconImage.gameObject.SetActive(false);
            }
        }

        // Enable/disable navigation buttons based on the quest status
        bool questInProgressOrCompleted = currentQuest != null && currentQuest.status != QuestEnums.QuestStatus.NotStarted;
        arrowLeftButton.interactable = !questInProgressOrCompleted && currentPageIndex > 0;
        arrowRightButton.interactable = !questInProgressOrCompleted && currentPageIndex < pages.Count - 1;
        startQuestButton.gameObject.SetActive(!questInProgressOrCompleted && currentQuest != null);
        turnInItemButton.gameObject.SetActive(questInProgressOrCompleted);
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
            questManager.AcceptQuest(currentQuest);  // This triggers the 'Initializing Quest...' log
            UpdateQuestUI();  // Refresh UI after starting the quest
            CloseQuestUI();  // Close the quest UI
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Quest {currentQuest.questName} is already started or completed.");
        }
    }

    // Method to handle quest icon hover event
    private void OnQuestIconHover(int index)
    {
        if (index < questIconImages.Count)
        {
            questIconImage.sprite = questIconImages[index];
        }
    }
}


[System.Serializable]
public class Page
{
    public string description;  // Local description text for the page
    public Sprite rewardIcon;  // Reward icon background for the page
    public Sprite questIcon;  // Quest icon for the page
    public Image questIconImage; // Reference to the Image component for the quest icon
    public MainQuest quest;  // Reference to the main quest associated with this page
    public List<Sprite> rewardIcons; // List of reward icons for the page
}
