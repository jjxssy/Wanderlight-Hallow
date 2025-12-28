using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawns a helper robot in the scene once the player's recorded respawn/death
/// count (stored in <see cref="PlayerPrefs"/> under "Respawn_Count") reaches
/// a configurable threshold. Intended as an accessibility/assist feature after
/// repeated failures.
/// </summary>
public class RobotSpawner : MonoBehaviour
{
    [Header("Robot Settings")]
    /// <summary>
    /// Prefab for the helper robot to instantiate when the threshold is met.
    /// </summary>
    [SerializeField] private GameObject helperRobotPrefab;

    /// <summary>
    /// Minimum number of recorded deaths/respawns required before spawning the helper.
    /// </summary>
    [SerializeField] private int deathThreshold = 3;

    /// <summary>
    /// On scene start, checks the current respawn count and spawns the helper if needed.
    /// </summary>
    void Start()
    {
        CheckAndSpawnRobot();
    }

    /// <summary>
    /// Reads "Respawn_Count" from <see cref="PlayerPrefs"/> and, if the value is
    /// greater than or equal to <see cref="deathThreshold"/>, instantiates
    /// <see cref="helperRobotPrefab"/> at this spawner's transform position.
    /// </summary>
    public void CheckAndSpawnRobot()
    {
        int respawnCount = PlayerPrefs.GetInt("Respawn_Count", 0);

        if (respawnCount >= deathThreshold)
        {
            Debug.Log("Death count is " + respawnCount + ". Spawning helper robot.");
            Instantiate(helperRobotPrefab, transform.position, Quaternion.identity);
        }
    }
}
