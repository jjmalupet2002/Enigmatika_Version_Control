using UnityEngine;
using UnityEngine.UI;

public class QuestLogController : MonoBehaviour
{
    // Public references to UI elements
    public Button questLogButton;
    public GameObject questUIPanel;
    public Button inventoryTabButton;
    public Button backButton;

    void Start()
    {     

        // Add listeners to the buttons to handle clicks
        if (questLogButton != null)
        {
            questLogButton.onClick.AddListener(OpenQuestLog);
        }
        if (inventoryTabButton != null)
        {
            inventoryTabButton.onClick.AddListener(CloseQuestLog);
        }
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseAllUI);
        }
    }

    // Method to open the quest log UI
    void OpenQuestLog()
    {
        if (questUIPanel != null)
        {
            questUIPanel.SetActive(true);
        }
    }

    // Method to close the quest log UI
    void CloseQuestLog()
    {
        if (questUIPanel != null)
        {
            questUIPanel.SetActive(false);
        }
    }

    // Method to close the quest log UI (used by the back button)
    void CloseAllUI()
    {
        if (questUIPanel != null)
        {
            questUIPanel.SetActive(false);
        }
    }
}
