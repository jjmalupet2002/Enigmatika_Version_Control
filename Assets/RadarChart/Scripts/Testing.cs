using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Testing : MonoBehaviour {

    [SerializeField] private UI_StatsRadarChart uiStatsRadarChart;

    private void Start() {
        Stats stats = new Stats(10, 2, 5, 10, 15);

        uiStatsRadarChart.SetStats(stats);
    }

}
