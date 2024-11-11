using UnityEngine;
using UnityEngine.UI;

public class InventoryToggle : MonoBehaviour
{
    public Button InventoryButton;
    public Button TalkButton;
    public Button InteractButton;
    public GameObject joystickCanvasGroup;
    public GameObject inventoryUI;
   
    public AudioSource openSound;
    public AudioSource closeSound;

    public GameObject QuestHolder;
    public GameObject QuestPopUpPrefab; // Reference to the QuestPopUp prefab
    public GameObject CriteriaUITemplatePrefab; // Reference to the CriteriaUITemplate prefab
    public GameObject QuestUITemplatePrefab; // Reference to the QuestUITemplate prefab

    private bool isInventoryOpen = false;
    private TalkandInteract talkandInteract;
    private Button activeButton;

    private GameObject questPopUpInstance; // Reference to the instantiated QuestPopUp
    private GameObject criteriaUITemplateInstance; // Reference to the instantiated CriteriaUITemplate
    private GameObject questUITemplateInstance; // Reference to the instantiated QuestUITemplate

    private void Start()
    {
        if (InventoryButton != null)
            InventoryButton.onClick.AddListener(OpenInventory);

        talkandInteract = FindObjectOfType<TalkandInteract>();
    }

    public void OpenInventory()
    {
        if (openSound != null)
            openSound.Play();

        inventoryUI.SetActive(true);
        SetControlsActive(false);

        GameStateManager.Instance.SetPlayerMovementState(false);
        var playerJoystick = FindObjectOfType<PlayerJoystickControl>();
        if (playerJoystick != null)
            playerJoystick.SetInputEnabled(false);

        isInventoryOpen = true;
    }

    public void CloseInventory()
    {
        if (closeSound != null)
            closeSound.Play();

        Invoke("DeactivateInventory", closeSound.clip.length);
    }

    private void DeactivateInventory()
    {
        inventoryUI.SetActive(false);
        SetControlsActive(true);

        if (Camera.main != null && Camera.main.enabled)
        {
            GameStateManager.Instance.SetPlayerMovementState(true);
            var playerJoystick = FindObjectOfType<PlayerJoystickControl>();
            if (playerJoystick != null)
                playerJoystick.SetInputEnabled(true);
        }

        isInventoryOpen = false;
    }

    private void SetControlsActive(bool isActive)
    {
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.SetActive(isActive);

        if (TalkButton != null && InteractButton != null)
        {
            if (isActive)
            {
                if (activeButton != null)
                {
                    activeButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (TalkButton.gameObject.activeSelf)
                {
                    activeButton = TalkButton;
                }
                else if (InteractButton.gameObject.activeSelf)
                {
                    activeButton = InteractButton;
                }

                TalkButton.gameObject.SetActive(false);
                InteractButton.gameObject.SetActive(false);
            }
        }

        if (InventoryButton != null)
            InventoryButton.gameObject.SetActive(isActive);

        // Toggle the instantiated UI prefab elements from quest system

        if (QuestHolder != null)
            QuestHolder.gameObject.SetActive(isActive);

        if (questPopUpInstance != null)
            questPopUpInstance.SetActive(isActive);

        if (criteriaUITemplateInstance != null)
            criteriaUITemplateInstance.SetActive(isActive);

        if (questUITemplateInstance != null)
            questUITemplateInstance.SetActive(isActive);
    }

    public void SetInventoryButtonActive(bool isActive)
    {
        if (InventoryButton != null)
        {
            InventoryButton.gameObject.SetActive(isActive);
        }
    }

    public bool IsInventoryOpen()
    {
        return isInventoryOpen; 
    }

    // Methods to instantiate the UI prefabs when needed
    public void InstantiateQuestPopUp()
    {
        if (QuestPopUpPrefab != null && questPopUpInstance == null)
        {
            questPopUpInstance = Instantiate(QuestPopUpPrefab);
        }
    }

    public void InstantiateCriteriaUITemplate()
    {
        if (CriteriaUITemplatePrefab != null && criteriaUITemplateInstance == null)
        {
            criteriaUITemplateInstance = Instantiate(CriteriaUITemplatePrefab);
        }
    }

    public void InstantiateQuestUITemplate()
    {
        if (QuestUITemplatePrefab != null && questUITemplateInstance == null)
        {
            questUITemplateInstance = Instantiate(QuestUITemplatePrefab);
        }
    }
}
