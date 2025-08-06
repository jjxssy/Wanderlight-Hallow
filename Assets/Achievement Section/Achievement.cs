using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievements/Achievement")]
public class Achievement : ScriptableObject
{
    [Tooltip("A unique ID for this achievement. E.g., 'kill_10_goblins'")]
    public string id;

    [Tooltip("The title that will be displayed to the player.")]
    public string title;

    [Tooltip("The description of what the player needs to do.")]
    [TextArea]
    public string description;

    [Tooltip("The icon to show when the achievement is locked.")]
    public Sprite lockedIcon;

    [Tooltip("The icon to show when the achievement is unlocked.")]
    public Sprite unlockedIcon;

    [Tooltip("How many points/kills/etc. are needed to unlock it.")]
    public int goal;

    [HideInInspector] public bool isUnlocked;
    [HideInInspector] public int currentProgress;
}