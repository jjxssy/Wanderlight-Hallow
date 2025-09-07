using TMPro;
using UnityEngine;

/// <summary>
/// Simple UI updater that displays live game statistics
/// (e.g., enemies killed and death count) every frame.
/// </summary>
public class TestingStatistics : MonoBehaviour
{
    /// <summary>
    /// UI text that shows the total number of enemies killed.
    /// </summary>
    [SerializeField] private TextMeshProUGUI enemiesKilled;

    /// <summary>
    /// UI text that shows the total number of player deaths.
    /// </summary>
    [SerializeField] private TextMeshProUGUI deathCound;

    /// <summary>
    /// Updates the UI texts each frame with the latest values from <see cref="StatisticsManager"/>.
    /// </summary>
    private void Update()
    {
        enemiesKilled.text = StatisticsManager.Get("enemiesKilled").ToString();
        deathCound.text = StatisticsManager.Get("deathCount").ToString();
    }
}
