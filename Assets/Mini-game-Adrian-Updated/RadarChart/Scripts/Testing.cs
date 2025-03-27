using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Testing : MonoBehaviour {

    [SerializeField] private UI_StatsRadarChart uiStatsRadarChart;

    private void Start() {
        // Example of how to check the answer
        string playerAnswer = "AUDACIOUS"; // This should be the player's inpu
    }
    

    private void UpdateStats(string category) {
        Stats stats = new Stats(0, 0, 0, 0, 0); // Initialize stats

        // Update stats based on the category
        switch (category) {
            case "focus":
                stats.SetStatAmount(Stats.Type.Attack, stats.GetStatAmount(Stats.Type.Attack) + 1); // Example increment
                break;
            // Add more cases for different categories if needed
            default:
                Debug.LogWarning("Unknown category: " + category);
                break;
        }

        // Set the updated stats to the radar chart
        uiStatsRadarChart.SetStats(stats);
    }
}