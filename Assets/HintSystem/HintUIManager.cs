using UnityEngine;
using UnityEngine.UI;

public class HintUIManager : MonoBehaviour
{
    public HintPointManager hintPointManager;
    public Text hintPointsText; // Use Text if you're not using TextMeshPro
    public Button hintButton;
    public GameObject hintPointIcon;

    private void OnEnable()
    {
        hintPointManager.onHintPointsUpdated.AddListener(UpdateHintPointsUI);
        hintPointManager.onHintButtonDisplay.AddListener(DisplayHintButton);
    }

    private void OnDisable()
    {
        hintPointManager.onHintPointsUpdated.RemoveListener(UpdateHintPointsUI);
        hintPointManager.onHintButtonDisplay.RemoveListener(DisplayHintButton);
    }

    private void Start()
    {
        hintButton.onClick.AddListener(OnHintButtonPressed);
        UpdateHintPointsUI();
    }

    private void UpdateHintPointsUI()
    {
        hintPointsText.text = hintPointManager.hintPointsSO.hintPoints.ToString();
    }

    public void DisplayHintButton()
    {
        hintButton.gameObject.SetActive(true);
    }

    private void OnHintButtonPressed()
    {
        hintPointManager.SubtractHintPoints(1); // Adjust the number of points subtracted as needed
        hintButton.gameObject.SetActive(false); // Optionally hide the button after use
    }
}