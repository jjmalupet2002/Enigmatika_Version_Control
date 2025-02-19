using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Save;
using CarterGames.Assets.SaveManager;
using System.Diagnostics;

public class QuestAcceptUI : MonoBehaviour
{
    public Button viewQuestButton;
    public GameObject questAcceptBackground;
    public GameObject QuestAcceptBlackbackground;
    public Button exitButton;
    public Button startQuestButton;
    public Text questNameText;
    public Text questDescriptionText;
    public Image rewardIconBackground;
    public Image questIconImage;
    public Button arrowLeftButton;
    public Button arrowRightButton;
    public Button turnInItemButton;
    public GameObject completeText;


    public List<MainQuest> mainQuests;
    public List<Page> pages;
    public List<Sprite> questIconImages;
    public List<Sprite> RewardIcons;

    private int currentPageIndex = 0;
    private MainQuest currentQuest;
    public QuestManager questManager;

    // Fields for required items and their associated reward items
    public string[] requiredItems = new string[4];
    public List<GameObject> wrongItemUIs; // List to hold references for each wrong item UI
    public Button wrongItemExitButton;
    public ItemEventHandler itemHandler;
    public InventoryManager inventoryManager;
    public QuestSavingSaveObject QuestSaveObject;


    void Start()
    {
        // Initialize UI elements
        questAcceptBackground.SetActive(false);
        turnInItemButton.gameObject.SetActive(false);
        turnInItemButton.interactable = false;
        startQuestButton.onClick.AddListener(StartSelectedQuest);
        exitButton.onClick.AddListener(CloseQuestUI);
        viewQuestButton.onClick.AddListener(OpenQuestUI);

        arrowLeftButton.onClick.AddListener(PreviousPage);
        arrowRightButton.onClick.AddListener(NextPage);

        // Initialize the exit button for wrongItemUI
        if (wrongItemExitButton != null)
        {
            wrongItemExitButton.onClick.AddListener(CloseWrongItemUI);
        }

        InitializePages();

        // Subscribe to the OnItemUsed event
        InventoryManager.Instance.OnItemUsed += OnItemUsed;

        // Subscribe to item events
        if (itemHandler != null)
        {
            itemHandler.item1Event += OnItem1Event;
            itemHandler.item2Event += OnItem2Event;
            itemHandler.item3Event += OnItem3Event;
            itemHandler.item4Event += OnItem4Event;
        }

        // Add listener for turn in button
        turnInItemButton.onClick.AddListener(TryTurnIn); // Listen for button click

    }

    private void OnEnable()
    {
        SaveEvents.OnSaveGame += SaveQuestAccept;
        SaveEvents.OnLoadGame += LoadQuestAccept;
    }

    private void OnDisable()
    {
        SaveEvents.OnSaveGame -= SaveQuestAccept;
        SaveEvents.OnLoadGame -= LoadQuestAccept;
    }

    private void SaveQuestAccept()
    {
        List<bool> questCompletionList = new List<bool>(); // Track quest completion separately
        foreach (var page in pages)
        {
            if (page.quest != null)
            {
                questCompletionList.Add(page.isQuestComplete); // Store separate completion status
            }
        }

        QuestSaveObject.questCompletionList.Value = questCompletionList; // Save only completion list
    }

    private void LoadQuestAccept()
    {
        List<bool> loadedQuestCompletion = QuestSaveObject.questCompletionList.Value;

        if (loadedQuestCompletion == null)
        {
            UnityEngine.Debug.LogWarning("No saved quest completion data found.");
            return;
        }

        for (int i = 0; i < loadedQuestCompletion.Count && i < pages.Count; i++)
        {
            if (pages[i].quest != null)
            {
                pages[i].isQuestComplete = loadedQuestCompletion[i]; // Restore completion status
            }
        }

        UpdateQuestUI();
    }

    // Event handler methods for each item
    private void OnItem1Event()
    {
        UnityEngine.Debug.Log("Item 1 event triggered.");
        // Handle the item 1 event (for example, reward the player)
    }

    private void OnItem2Event()
    {
        UnityEngine.Debug.Log("Item 2 event triggered.");
        // Handle the item 2 event (for example, reward the player)
    }

    private void OnItem3Event()
    {
        UnityEngine.Debug.Log("Item 3 event triggered.");
        // Handle the item 3 event (for example, reward the player)
    }

    private void OnItem4Event()
    {
        UnityEngine.Debug.Log("Item 4 event triggered.");
        // Handle the item 4 event (for example, reward the player)
    }

    void InitializePages()
    {
        if (mainQuests.Count == 0 || pages.Count == 0 || wrongItemUIs.Count < 4)
        {
            UnityEngine.Debug.LogWarning("No available quests or pages to display.");
            return;
        }

        for (int i = 0; i < pages.Count; i++)
        {
            if (i < mainQuests.Count)
            {
                pages[i].quest = mainQuests[i];
                pages[i].questIcon = questIconImages[i % questIconImages.Count];
                pages[i].rewardIcon = RewardIcons[i % RewardIcons.Count];

                // Add event listeners for hover events
                int index = i;
                pages[i].questIconImage = questIconImage;
                pages[i].questIconImage.GetComponent<Button>().onClick.AddListener(() => OnQuestIconHover(index));

                pages[i].rewardIconBackground = rewardIconBackground;
                pages[i].rewardIconBackground.GetComponent<Button>().onClick.AddListener(() => OnQuestIconHover(index));

                // Assign the corresponding wrongItemUI to each page
                pages[i].wrongItemUI = wrongItemUIs[i];
            }
            else
            {
                pages[i].quest = null;
            }
        }

        UpdateQuestUI();
    }

    void OpenQuestUI()
    {
        questAcceptBackground.SetActive(true);
        QuestAcceptBlackbackground.SetActive(true);

        // Find the first page with a quest that is in progress or not yet completed
        currentPageIndex = pages.FindIndex(page => page.quest != null && page.quest.status != QuestEnums.QuestStatus.NotStarted && !page.isQuestComplete);

        // If no active quest page is found, default to the first page
        if (currentPageIndex == -1)
        {
            currentPageIndex = 0;
        }

        UpdateQuestUI();
    }

    void CloseQuestUI()
    {
        questAcceptBackground.SetActive(false);
        QuestAcceptBlackbackground.SetActive(false);
        turnInItemButton.interactable = false;
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

    private void UpdateQuestUI()
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
            questDescriptionText.text = currentQuest.questDescription;
            rewardIconBackground.sprite = currentPage.rewardIcon;
            questIconImage.sprite = currentPage.questIcon;
        }
        else
        {
            questNameText.text = "No Quest";
            questDescriptionText.text = "No description available.";
            rewardIconBackground.sprite = null;
            questIconImage.sprite = null;
        }

        bool questInProgressOrCompleted = currentQuest != null && currentQuest.status != QuestEnums.QuestStatus.NotStarted;
        bool questPageCompleted = currentPage.isQuestComplete;

        // If the quest is completed, ensure the arrows remain interactable
        arrowLeftButton.interactable = !questInProgressOrCompleted && currentPageIndex > 0 || questPageCompleted;
        arrowRightButton.interactable = !questInProgressOrCompleted && currentPageIndex < pages.Count - 1 || questPageCompleted;

        // Ensure buttons reflect the correct state based on quest completion
        startQuestButton.gameObject.SetActive(!questInProgressOrCompleted && currentQuest != null && !questPageCompleted);
        turnInItemButton.gameObject.SetActive(questInProgressOrCompleted && !questPageCompleted);
        completeText.SetActive(questPageCompleted);
    }

    private void StartSelectedQuest()
    {
        if (currentQuest == null)
        {
            UnityEngine.Debug.LogWarning("No quest is selected to start.");
            return;
        }

        if (currentQuest.status == QuestEnums.QuestStatus.NotStarted)
        {
            questManager.AcceptQuest(currentQuest);
            UpdateQuestUI();
            CloseQuestUI();
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Quest {currentQuest.questName} is already started or completed.");
        }
    }

    private void OnQuestIconHover(int index)
    {
        if (index < questIconImages.Count)
        {
            questIconImage.sprite = questIconImages[index];
        }
    }

    // Add a class-level variable to store the item used
    private ItemData currentItem;

    // Flag to track if the correct item has been used
    private bool hasUsedItem = false;

    // Flag to track if the correct comparison item is used
    private bool isComparisonItem = false;

    // Flag to track if the turn-in button is pressed
    private bool isTurnInButtonClicked = false;

    // Method to handle button click
    private void OnTurnInButtonClick()
    {
        UnityEngine.Debug.Log("Turn-In button clicked.");

        if (hasUsedItem)  // Only allow turn-in if an item has been used
        {
            isTurnInButtonClicked = true;  // Set the flag to true
            UnityEngine.Debug.Log("Turn-In button click registered.");
        }
        else
        {
            UnityEngine.Debug.Log("Turn-In button click ignored because no item has been used.");
        }

        TryTurnIn();  // Always call TryTurnIn, regardless of whether an item has been used or not
    }

    // Method to handle item usage
    public void OnItemUsed(ItemData item)
    {
        if (item == null)
        {
            UnityEngine.Debug.Log("No item passed to OnItemUsed.");
            return;
        }

        UnityEngine.Debug.Log($"Item Used: {item.keyId}, Required Item: {requiredItems[currentPageIndex]}");
        turnInItemButton.interactable = true; // Enable the button component

        // Check if the item keyId matches the required item for the current page
        if (item.keyId == requiredItems[currentPageIndex])
        {
            // Mark that the correct item has been used and store the item
            isComparisonItem = true;
            hasUsedItem = true;  // Correct item used
            currentItem = item; // Store the used item

            UnityEngine.Debug.Log("Correct item used, proceeding to turn in.");
        }
        else
        {
            // Log the wrong item usage
            // Ensure hasUsedItem is still set to true when a wrong item is used
            hasUsedItem = true;  // This allows triggering the wrong item UI when the button is clicked
        }
    }

    // Method to check both conditions (item used and button clicked) and perform the action
    public void TryTurnIn()
    {
        UnityEngine.Debug.Log($"TryTurnIn called with states - isComparisonItem: {isComparisonItem}, isTurnInButtonClicked: {isTurnInButtonClicked}, hasUsedItem: {hasUsedItem}");

        // Ensure both conditions are met for the correct item
        if (isComparisonItem)
        {
            // Mark quest as complete and trigger necessary actions
            QuestComplete(currentPageIndex);

            // Trigger the corresponding event using HandleItemEvent method
            itemHandler.HandleItemEvent(currentPageIndex);

            // Pass the stored item
            StartCoroutine(DeleteItemAfterDelay(currentItem, 1.5f));

            UnityEngine.Debug.Log("Quest Completed!");

            // Reset flags after turn-in attempt
            ResetFlags();
        }
        else
        {
            // Show the wrong item UI for the current page
            GameObject currentWrongItemUI = wrongItemUIs[currentPageIndex];
            if (currentWrongItemUI != null)
            {
                currentWrongItemUI.SetActive(true);
                wrongItemExitButton.gameObject.SetActive(true);
            }
            else
            {
                UnityEngine.Debug.LogWarning("No wrongItemUI assigned for page " + currentPageIndex);
            }

            // Reset flags after turn-in attempt, without resetting turn-in button immediately
            ResetFlags();
        }
    }

    // Coroutine to delete item after a delay
    private IEnumerator DeleteItemAfterDelay(ItemData item, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the delay before deleting the item

        // Delete the item from inventory
        inventoryManager.DeleteItem(item);

    }


    // Helper method to reset flags
    private void ResetFlags()
    {
        // Reset flags that track the turn-in state
        isTurnInButtonClicked = false;  // Reset the button press flag
        isComparisonItem = false;       // Reset the comparison check flag
        hasUsedItem = false;            // Reset the item usage check flag
    }


    private void QuestComplete(int questIndex)
    {
        Page currentPage = pages[questIndex];

        // Ensure quest is not null
        if (currentPage.quest != null)
        {
            // Update quest description and status
            currentPage.quest.questDescription = "Quest Completed!\nPortal room key received!";
            currentPage.quest.status = QuestEnums.QuestStatus.Completed;

            // Mark the page as quest complete
            currentPage.isQuestComplete = true;
            SaveQuestAccept(); // Save the completion status
        }

        // Disable the turn-in button for the current page
        turnInItemButton.gameObject.SetActive(false);
        completeText.gameObject.SetActive(true);

        // Update the UI with the updated quest information
        UpdateQuestUI();  // Refresh the UI with updated quest info

        // Enable the arrows' interactability as the quest is now completed
        arrowLeftButton.interactable = true;
        arrowRightButton.interactable = true;
    }


    // Method to close all wrongItemUI pages
    private void CloseWrongItemUI()
    {
        foreach (var wrongItemUI in wrongItemUIs)
        {
            if (wrongItemUI != null && wrongItemUI.activeSelf)
            {
                wrongItemUI.SetActive(false);
                wrongItemExitButton.gameObject.SetActive(false);
                UnityEngine.Debug.Log($"Closed wrongItemUI: {wrongItemUI.name}");
                ResetFlags();
            }
        }
    }
}

[System.Serializable]
public class Page
{
    public string description;  // Local description text for the page
    public Image rewardIconBackground;
    public Sprite rewardIcon;  // Reward icon background for the page
    public Sprite questIcon;  // Quest icon for the page
    public Image questIconImage; // Reference to the Image component for the quest icon
    public MainQuest quest; // Local quest variable for the page
    public bool isQuestComplete;
    public GameObject wrongItemUI;
}