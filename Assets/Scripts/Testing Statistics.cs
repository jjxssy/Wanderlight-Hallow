using TMPro;
using UnityEngine;

public class TestingStatistics : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemiesKilled;
    [SerializeField] private TextMeshProUGUI deathCound;

    void Update()
    {
        enemiesKilled.text = StatisticsManager.Get("enemiesKilled").ToString();
        deathCound.text = StatisticsManager.Get("deathCount").ToString();
    }
}
