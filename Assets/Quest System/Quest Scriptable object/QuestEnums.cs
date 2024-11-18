using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEnums : MonoBehaviour
{
    public enum QuestType
    {
        Main,
        Criteria
    }

    public enum QuestCriteriaType
    {
        Find,
        Explore,
        Escape,
        UnlockSolve
    }

    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}

