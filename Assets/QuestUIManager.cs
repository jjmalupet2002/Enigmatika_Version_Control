using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public Text questNameText;
    public Text criteriaStatusText;
    public Text questCompletionText;  // Add a new Text field for completion message

    private QuestManager questManager;

    private CanvasGroup questNameCanvasGroup;
    private CanvasGroup criteriaStatusCanvasGroup;
    private CanvasGroup questCompletionCanvasGroup;

    void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        // Get CanvasGroup components
        questNameCanvasGroup = questNameText.GetComponent<CanvasGroup>();
        criteriaStatusCanvasGroup = criteriaStatusText.GetComponent<CanvasGroup>();
        questCompletionCanvasGroup = questCompletionText.GetComponent<CanvasGroup>();

        // Ensure CanvasGroup components are on the quest text objects
        if (questNameCanvasGroup == null) questNameCanvasGroup = questNameText.gameObject.AddComponent<CanvasGroup>();
        if (criteriaStatusCanvasGroup == null) criteriaStatusCanvasGroup = criteriaStatusText.gameObject.AddComponent<CanvasGroup>();
        if (questCompletionCanvasGroup == null) questCompletionCanvasGroup = questCompletionText.gameObject.AddComponent<CanvasGroup>();

        // Set initial alpha to 0 (invisible)
        questNameCanvasGroup.alpha = 0f;
        criteriaStatusCanvasGroup.alpha = 0f;
        questCompletionCanvasGroup.alpha = 0f;

        // Subscribe to events
        questManager.OnQuestAcceptedEvent += UpdateQuestUI;
        questManager.OnQuestCompletedEvent += UpdateQuestUI;
        questManager.OnNextCriteriaStartedEvent += UpdateQuestUIForNextCriteria;
    }

    // Update UI when a quest is accepted or completed
    void UpdateQuestUI(MainQuest quest)
    {
        if (quest.status == QuestEnums.QuestStatus.InProgress || quest.status == QuestEnums.QuestStatus.Completed)
        {
            // Only show quest name and criteria text if the quest is in progress
            if (quest.status == QuestEnums.QuestStatus.InProgress)
            {
                StartCoroutine(FadeInQuestNameText(quest.questName));  // Fade in quest name if the quest is in progress
                StartCoroutine(FadeInCriteriaText());
            }
            else
            {
                questNameText.gameObject.SetActive(false);  // Hide quest name when the quest is completed
            }

            // Show the criteria text only if the quest is in progress
            if (quest.status == QuestEnums.QuestStatus.InProgress)
            {
                criteriaStatusText.gameObject.SetActive(true);
                criteriaStatusText.text = "";  // Reset the criteria text

                // Iterate through the criteria and display only the InProgress criteria
                foreach (var criteria in quest.questCriteriaList)
                {
                    if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                    {
                        criteriaStatusText.text += $"{criteria.criteriaName}";
                    }
                }
            }
            else
            {
                // Quest is completed, so display the completion text
                StartCoroutine(ShowQuestCompletionText());
            }
        }
        else
        {
            questNameText.gameObject.SetActive(false);
            criteriaStatusText.gameObject.SetActive(false);
            questCompletionText.gameObject.SetActive(false);
        }

        // Hide criteria when completed
        if (quest.status == QuestEnums.QuestStatus.Completed)
        {
            criteriaStatusText.gameObject.SetActive(false);
        }
    }

    // Coroutine to show the quest completion text after the quest is completed
    private IEnumerator ShowQuestCompletionText()
    {
        // Immediately show the quest completion text
        questCompletionText.gameObject.SetActive(true);
        yield return FadeInQuestCompletionText();  // Fade in quest completion text

        // Wait for 1 second before fading out the quest completion text
        yield return new WaitForSeconds(1f);
        StartCoroutine(HideQuestCompletionText());
    }


    // Coroutine to fade in the quest name text
    private IEnumerator FadeInQuestNameText(string questName)
    {
        questNameText.text = questName;
        questNameText.gameObject.SetActive(true);  // Ensure text is active
        float duration = 0.5f;
        float startAlpha = questNameCanvasGroup.alpha;
        float endAlpha = 1f;

        // Fade in
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            questNameCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }
        questNameCanvasGroup.alpha = endAlpha;  // Ensure it ends fully visible

        // Now that the fade-in is complete, start the fade-out
        yield return new WaitForSeconds(2f);  // Wait for 2 seconds before starting fade-out
        StartCoroutine(FadeOutQuestNameText());
    }

    // Coroutine to fade in the criteria text alongside the quest name
    private IEnumerator FadeInCriteriaText()
    {
        float duration = 0.5f;  // Duration of fade-in
        float startAlpha = criteriaStatusCanvasGroup.alpha;
        float endAlpha = 1f;

        // Fade in
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            criteriaStatusCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }
        criteriaStatusCanvasGroup.alpha = endAlpha;  // Ensure it ends fully visible
    }

    // Coroutine to fade in the quest completion text
    private IEnumerator FadeInQuestCompletionText()
    {
        float duration = 0.5f;  // Duration of fade-in
        float startAlpha = questCompletionCanvasGroup.alpha;
        float endAlpha = 1f;

        // Fade in
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            questCompletionCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }
        questCompletionCanvasGroup.alpha = endAlpha;  // Ensure it ends fully visible
    }

    // Coroutine to fade out the quest completion text after 2 seconds
    private IEnumerator HideQuestCompletionText()
    {
        yield return new WaitForSeconds(2f);  // Wait for 2 seconds

        // Fade out
        float duration = 0.7f;  // Duration of fade-out
        float startAlpha = questCompletionCanvasGroup.alpha;
        float endAlpha = 0f;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            questCompletionCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }
        questCompletionCanvasGroup.alpha = endAlpha;  // Ensure it ends fully transparent
        questCompletionText.gameObject.SetActive(false);  // Finally, deactivate the text
    }

    // Coroutine to fade out the quest name text after 2 seconds
    private IEnumerator FadeOutQuestNameText()
    {
        yield return new WaitForSeconds(2f);  // Wait for 2 seconds

        // Fade out
        float duration = 0.7f;  // Duration of fade-out
        float startAlpha = questNameCanvasGroup.alpha;
        float endAlpha = 0f;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            questNameCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }
        questNameCanvasGroup.alpha = endAlpha;  // Ensure it ends fully transparent
        questNameText.gameObject.SetActive(false);  // Finally, deactivate the text
    }


    // New method to handle UI update when the next criteria starts
    void UpdateQuestUIForNextCriteria(MainQuest quest)
    {
        // Check if quest is in progress and there are criteria to show
        if (quest.status == QuestEnums.QuestStatus.InProgress)
        {
            // Reset the criteria text
            criteriaStatusText.text = "";

            // Iterate through the criteria and display only the InProgress criteria
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                {
                    criteriaStatusText.text += $"{criteria.criteriaName}";
                }
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        questManager.OnQuestAcceptedEvent -= UpdateQuestUI;
        questManager.OnQuestCompletedEvent -= UpdateQuestUI;
        questManager.OnNextCriteriaStartedEvent -= UpdateQuestUIForNextCriteria;
    }
}
