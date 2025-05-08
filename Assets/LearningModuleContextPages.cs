using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LearningModuleContextPages : MonoBehaviour
{
    [Header("Pages")]
    public GameObject[] pages = new GameObject[6]; // Adjusted to 6 pages since you originally mentioned 6

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button prevButton;
    public Button exitButton; // <-- Reference in inspector (should be disabled by default)

    private int currentPageIndex = 0;

    private void Start()
    {
        // Only the first page is active at the start
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == 0);
        }

        nextButton.onClick.AddListener(GoToNextPage);
        prevButton.onClick.AddListener(GoToPreviousPage);

        exitButton.gameObject.SetActive(false); // Ensure it's hidden at the start

        UpdateNavigationUI();
    }

    private void GoToNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            pages[currentPageIndex].SetActive(false);
            currentPageIndex++;
            pages[currentPageIndex].SetActive(true);
            UpdateNavigationUI();
        }
    }

    private void GoToPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            pages[currentPageIndex].SetActive(false);
            currentPageIndex--;
            pages[currentPageIndex].SetActive(true);
            UpdateNavigationUI();
        }
    }

    private void UpdateNavigationUI()
    {
        prevButton.interactable = currentPageIndex > 0;

        bool isLastPage = currentPageIndex == pages.Length - 1;

        nextButton.gameObject.SetActive(!isLastPage);
        exitButton.gameObject.SetActive(isLastPage);
    }
}
