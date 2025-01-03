using UnityEngine;
using System.Collections.Generic;
using static QuestEnums;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class QuestSystem : ScriptableObject
{
    public string questName;
    public QuestType questType; // Main or Criteria
    public QuestCriteriaType criteriaType; // Find, Explore, Escape, UnlockSolve
    public QuestStatus status;
    public string description;
    public List<string> objectives; // List of objectives for the quest
    public int rewardPoints; // Points or items rewarded upon completion
}
