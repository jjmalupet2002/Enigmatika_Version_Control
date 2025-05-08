using UnityEngine;
using UnityEngine.UI;

public class QuestLogController : MonoBehaviour
{
    // Panels
    public GameObject questUIPanel;
    public GameObject inventoryPanel;
    public GameObject caseFilePanel;

    // Buttons
    public Button questLogButton;       // Opens Quest Log from Inventory
    public Button inventoryTabButton;   // Opens Inventory from Quest Log
    public Button backButton;           // Closes Quest Log

    public Button caseFileButton;       // Opens Case File from anywhere
    public Button caseFileBackButton;   // Closes Case File

    public Button questFromCaseFileButton;       // Opens Quest Log from Case File
    public Button inventoryFromCaseFileButton;   // Opens Inventory from Case File
    public Button caseFileFromInventoryButton;   // NEW: Opens Case File from Inventory

    void Start()
    {
        // Quest Log buttons
        if (questLogButton != null)
            questLogButton.onClick.AddListener(OpenQuestLog);

        if (inventoryTabButton != null)
            inventoryTabButton.onClick.AddListener(OpenInventory);

        if (backButton != null)
            backButton.onClick.AddListener(CloseQuestLog);

        // Case File buttons
        if (caseFileButton != null)
            caseFileButton.onClick.AddListener(OpenCaseFile);

        if (caseFileBackButton != null)
            caseFileBackButton.onClick.AddListener(CloseCaseFile);

        if (questFromCaseFileButton != null)
            questFromCaseFileButton.onClick.AddListener(OpenQuestLog);

        if (inventoryFromCaseFileButton != null)
            inventoryFromCaseFileButton.onClick.AddListener(OpenInventory);

        if (caseFileFromInventoryButton != null)
            caseFileFromInventoryButton.onClick.AddListener(OpenCaseFile); // Assign new action
    }

    // ----------------------
    // Panel Logic
    void OpenQuestLog()
    {
        questUIPanel?.SetActive(true);
        inventoryPanel?.SetActive(false);
        caseFilePanel?.SetActive(false);
    }

    void CloseQuestLog()
    {
        questUIPanel?.SetActive(false);
    }

    void OpenInventory()
    {
        inventoryPanel?.SetActive(true);
        questUIPanel?.SetActive(false);
        caseFilePanel?.SetActive(false);
    }

    void OpenCaseFile()
    {
        caseFilePanel?.SetActive(true);
        questUIPanel?.SetActive(false);
        inventoryPanel?.SetActive(false);
    }

    void CloseCaseFile()
    {
        caseFilePanel?.SetActive(false);
    }
}
