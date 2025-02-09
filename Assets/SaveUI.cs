using UnityEngine;
using UnityEngine.UI;
using CarterGames.Assets.SaveManager;
using System.Collections;

public class SaveUI : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private GameObject gameSavedText;
    [SerializeField] private GameObject SaveBtnBG;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject autoSaveIcon;

    private bool wasDisabledEarlier = false;

    private void Awake()
    {
        SaveEvents.OnSaveGame += HandleGameSave;
        SaveEvents.OnSaveGame += StartAutoSaveAnimation; //Subscribe AutoSave Animation
        saveButton.onClick.AddListener(SaveGame);
    }

    private void OnDestroy()
    {
        SaveEvents.OnSaveGame -= HandleGameSave;
        SaveEvents.OnSaveGame -= StartAutoSaveAnimation; //Unsubscribe to prevent memory leaks
        saveButton.onClick.RemoveListener(SaveGame);
    }

    private void SaveGame()
    {
        SaveEvents.SaveGame();
    }

    private void HandleGameSave()
    {
        saveButton.gameObject.SetActive(false);
        SaveBtnBG.SetActive(false);
        gameSavedText.SetActive(true);
        wasDisabledEarlier = true;
    }

    public void OnPauseMenuOpened()
    {
        if (wasDisabledEarlier)
        {
            PauseBecomesActive();
        }
    }

    private void PauseBecomesActive()
    {
        saveButton.gameObject.SetActive(true);
        SaveBtnBG.SetActive(true);
        gameSavedText.SetActive(false);
        wasDisabledEarlier = false;
    }

    //Automatically starts the auto-save animation when any script calls SaveEvents.SaveGame()
    private void StartAutoSaveAnimation()
    {
        StartCoroutine(ShowAutoSaveIcon());
    }

    private IEnumerator ShowAutoSaveIcon()
    {
        autoSaveIcon.SetActive(true); // Enable the icon
        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            autoSaveIcon.transform.Rotate(Vector3.forward, -360 * Time.deltaTime); // Rotate clockwise
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        autoSaveIcon.SetActive(false); // Disable after 4 seconds
    }
}
