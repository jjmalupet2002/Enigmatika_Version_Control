using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class QuestAcceptUI : MonoBehaviour
{
    public Button viewQuestButton;
    public GameObject questAcceptBackground;
    public Button exitButton;
    public Button startQuestButton;
    public Text questNameText;
    public Text questDescriptionText;
    public Image rewardIconBackground;
    public Image questIconImage;
    public Button arrowLeftButton;
    public Button arrowRightButton;
    public Button turnInItemButton;

    public List<MainQuest> mainQuests;
    public List<Page> pages;
    public List<Sprite> questIconImages;
    public List<Sprite> RewardIcons;

    private int currentPageIndex = 0;
    private MainQuest currentQuest;
    public QuestManager questManager;

    // Fields for required items and their associated reward items
    public string[] requiredItems = new string[4];
    private GameObject wrongItemUI;
    public ItemRewardGiver itemHandler;  // Reference to the ItemRewardGiver

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

        InitializePages();

        // Subscribe to the OnItemUsed event
        InventoryManager.Instance.OnItemUsed += OnItemUsed;
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
                pages[i].questIcon = questIconImages[i % questIconImages.Count];
                pages[i].rewardIcon = RewardIcons[i % RewardIcons.Count];

                // Add event listeners for hover events
                int index = i;
                pages[i].questIconImage = questIconImage;
                pages[i].questIconImage.GetComponent<Button>().onClick.AddListener(() => OnQuestIconHover(index));

                pages[i].rewardIconBackground = rewardIconBackground;
                pages[i].rewardIconBackground.GetComponent<Button>().onClick.AddListener(() => OnQuestIconHover(index));
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
        currentPageIndex = 0;
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
        arrowLeftButton.interactable = !questInProgressOrCompleted && currentPageIndex > 0;
        arrowRightButton.interactable = !questInProgressOrCompleted && currentPageIndex < pages.Count - 1;
        startQuestButton.gameObject.SetActive(!questInProgressOrCompleted && currentQuest != null);
        turnInItemButton.gameObject.SetActive(questInProgressOrCompleted);
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

    private void OnItemUsed(ItemData item)
    {
        if (item != null)
        {
            // Check if the item keyId matches the required item for the current page
            if (item.keyId == requiredItems[currentPageIndex])
            {
                // Fire event for quest completion
                QuestComplete(currentPageIndex);            

                // Trigger the corresponding event based on the currentPageIndex
                switch (currentPageIndex)
                {
                    case 0:
                        itemHandler.item1Event?.Invoke();
                        break;
                    case 1:
                        itemHandler.item2Event?.Invoke();
                        break;
                    case 2:
                        itemHandler.item3Event?.Invoke();
                        break;
                    case 3:
                        itemHandler.item4Event?.Invoke();
                        break;
                    default:
                        UnityEngine.Debug.LogWarning("No matching event for this page index.");
                        break;
                }
            }
            else
            {
                wrongItemUI.SetActive(true); // Show wrong item UI
            }
        }
    }

    private void QuestComplete(int questIndex)
    {
        Page currentPage = pages[questIndex];
        currentPage.description = "Quest Complete!";
        UpdateQuestUI();
        arrowLeftButton.interactable = true;
        arrowRightButton.interactable = true;

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
    public MainQuest quest;  // Reference to the main quest associated with this page
}
