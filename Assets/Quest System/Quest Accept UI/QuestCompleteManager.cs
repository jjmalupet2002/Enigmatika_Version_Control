using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTurnInUIManager : MonoBehaviour
{
    // Header 1: Quest Screen Settings
    [Header("Quest Screen Settings")]
    public GameObject questScreen;
    public Button cancelQuestButton;
    public Button exitQuestButton;

    // Header 2: Wrong Item Screen
    [Header("Wrong Item Screen")]
    public GameObject wrongItemScreen;
    public Button turnInButton;
    public Button exitWrongItemButton; // New exit button for wrong item screen

    // Header 3: Hint UI
    [Header("Hint UI")]
    public GameObject hintUI;
    public Button hintButton;
    public Button exitHintButton;

    // Header 4: Other UI
    [Header("Other UI")]
    public GameObject correctItemScreen;
    public Button openQuestScreenButton;

    // Header 5: Additional UI Elements
    [Header("Additional UI Elements")]
    public GameObject inventoryButton;
    public GameObject joystickCanvasGroup;
    public GameObject TalkButton;

    private enum ActiveScreen { None, Quest, WrongItem, CorrectItem }
    private ActiveScreen currentScreen = ActiveScreen.None;

    void Start()
    {
        // Add listeners to buttons
        openQuestScreenButton.onClick.AddListener(OpenQuestScreen);
        cancelQuestButton.onClick.AddListener(CancelQuest);
        exitQuestButton.onClick.AddListener(CloseCurrentScreen);
        hintButton.onClick.AddListener(OpenHintUI);
        exitHintButton.onClick.AddListener(CloseHintUI);
        turnInButton.onClick.AddListener(TurnInWrongItem);
        exitWrongItemButton.onClick.AddListener(CloseWrongItemScreen); // Listener for the new exit button
    }

    void OpenQuestScreen()
    {
        if (currentScreen == ActiveScreen.None)
        {
            questScreen.SetActive(true);
            hintButton.gameObject.SetActive(true);
            openQuestScreenButton.gameObject.SetActive(false); // Deactivate the open quest screen button
            SetAdditionalUIElementsActive(false);
            currentScreen = ActiveScreen.Quest;
        }
    }

    void CancelQuest()
    {
        wrongItemScreen.SetActive(true);
        questScreen.SetActive(false);
        currentScreen = ActiveScreen.WrongItem;
    }

    public void CloseCurrentScreen()
    {
        switch (currentScreen)
        {
            case ActiveScreen.Quest:
                questScreen.SetActive(false);
                break;
            case ActiveScreen.WrongItem:
                wrongItemScreen.SetActive(false);
                break;
            case ActiveScreen.CorrectItem:
                correctItemScreen.SetActive(false);
                break;
        }
        hintButton.gameObject.SetActive(false);
        openQuestScreenButton.gameObject.SetActive(true); // Reactivate the open quest screen button
        SetAdditionalUIElementsActive(true);
        currentScreen = ActiveScreen.None;
    }

    void OpenHintUI()
    {
        hintUI.SetActive(true);
        hintButton.gameObject.SetActive(false);
    }

    void CloseHintUI()
    {
        hintUI.SetActive(false);
        hintButton.gameObject.SetActive(true);
    }

    public void TurnInWrongItem()
    {
        if (questScreen.activeSelf)
        {
            questScreen.SetActive(false);
            hintButton.gameObject.SetActive(false);
        }
        else if (wrongItemScreen.activeSelf)
        {
            wrongItemScreen.SetActive(false);
            hintButton.gameObject.SetActive(false);

        }
        correctItemScreen.SetActive(true);
        currentScreen = ActiveScreen.CorrectItem;
    }

    void CloseWrongItemScreen()
    {
        wrongItemScreen.SetActive(false);
        questScreen.SetActive(false);
        hintButton.gameObject.SetActive(false);
        openQuestScreenButton.gameObject.SetActive(true); // Reactivate the open quest screen button
        SetAdditionalUIElementsActive(true);
        currentScreen = ActiveScreen.None;
    }

    public void CloseCorrectItemScreen()
    {
        correctItemScreen.SetActive(false);
        hintButton.gameObject.SetActive(false);
        openQuestScreenButton.gameObject.SetActive(true); // Reactivate the open quest screen button
        SetAdditionalUIElementsActive(true);
        currentScreen = ActiveScreen.None;
    }

    void SetAdditionalUIElementsActive(bool isActive)
    {
        inventoryButton.SetActive(isActive);
        TalkButton.SetActive(isActive);
        joystickCanvasGroup.SetActive(isActive);
       
    }
}
