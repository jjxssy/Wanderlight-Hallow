using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotSpawner : MonoBehaviour
{
    [Header("Robot Settings")]
    [SerializeField] private GameObject helperRobotPrefab;
    [SerializeField] private int deathThreshold = 3;

    void Start()
    {
        CheckAndSpawnRobot();
    }

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