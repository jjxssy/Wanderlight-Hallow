using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines a single achievement, including its ID, visuals, goal, and progress state.
/// Instances are created as ScriptableObjects and can be edited in the Unity Inspector.
/// </summary>
[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievements/Achievement")]
public class Achievement : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("A unique ID for this achievement. E.g., 'kill_10_goblins'.")]
    [SerializeField] private string id;

    [Tooltip("The title displayed to the player.")]
    [SerializeField] private string title;

    [Tooltip("The description of what the player needs to do.")]
    [TextArea]
    [SerializeField] private string description;

    [Header("Visuals")]
    [Tooltip("The icon shown when the achievement is locked.")]
    [SerializeField] private Sprite lockedIcon;

    [Tooltip("The icon shown when the achievement is unlocked.")]
    [SerializeField] private Sprite unlockedIcon;

    [Header("Progress")]
    [Tooltip("How many points/kills/etc. are needed to unlock it.")]
    [SerializeField] private int goal;

    [HideInInspector] [SerializeField] private bool isUnlocked;
    [HideInInspector] [SerializeField] private int currentProgress;


}