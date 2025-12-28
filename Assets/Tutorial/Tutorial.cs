using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Simple, stage-based tutorial flow controller.
/// Shows a sequence of dialogues as the player:
/// 1) Moves with WASD,
/// 2) Picks up a test item,
/// 3) Acquires a test skill.
/// Each milestone advances the tutorial and triggers the next dialogue set.
/// </summary>
public class Tutorial : MonoBehaviour
{
    /// <summary>Dialogue manager used to display tutorial text.</summary>
    [SerializeField] private DialogManager dialogManager;

    /// <summary>First set: prompts the player to move (WASD).</summary>
    [TextArea(3,10)]
    [SerializeField] private string[] MovenentDialogues;

    /// <summary>Shown after the player moves at least once.</summary>
    [TextArea(3, 10)]
    [SerializeField] private string[] MovementCompleteDialogues;

    /// <summary>Reference to an item in the world the player must pick up.</summary>
    [SerializeField] private ItemWorld testItem;

    /// <summary>Shown after the player picks up the test item (inventory intro).</summary>
    [TextArea(3, 10)]
    [SerializeField] private string[] InventoryDialogues;

    /// <summary>Reference to a world skill the player must acquire.</summary>
    [SerializeField] private WorldSkill testSkill;

    /// <summary>Shown after the player acquires the test skill (skills intro).</summary>
    [TextArea(3, 10)]
    [SerializeField] private string[] skillDialogues;

    /// <summary>Keys considered valid movement input for the first milestone.</summary>
    private KeyCode[] movementKeys = { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D };

    /// <summary>
    /// Tutorial progression state:
    /// 0 = pre-start, 1 = waiting for movement, 2 = waiting for item pickup,
    /// 3 = waiting for skill pickup, 4 = completed.
    /// </summary>
    private int dialogState = 0;

    /// <summary>Set true after the initial movement dialogue is shown.</summary>
    private bool firstTextShown = false;

    /// <summary>
    /// Kicks off the tutorial flow by showing the initial movement dialogue
    /// after a short delay.
    /// </summary>
    void Start()
    {
        StartCoroutine(StartDialogue());
        // PlayerPrefs.GetInt("DialogueState", 0);
    }

    /// <summary>
    /// Monitors player input and world state to advance the tutorial:
    /// - If player moved (WASD) after the first prompt, show completion dialogue.
    /// - If item reference becomes null (picked up), show inventory dialogue.
    /// - If skill reference becomes null (acquired), show skill dialogue and complete.
    /// </summary>
    void Update()
    {
        if (firstTextShown && movementKeys.Any(Input.GetKeyDown) && dialogState == 1)
        {
            dialogManager.ShowDialog(MovementCompleteDialogues);
            dialogState = 2;
            // PlayerPrefs.SetInt("DialogueState", 2);
        }

        if (dialogState == 2 && testItem == null)
        {
            dialogManager.ShowDialog(InventoryDialogues);
            dialogState = 3;
            // PlayerPrefs.SetInt("DialogueState", 3);
        }

        if (dialogState == 3 && testSkill == null)
        {
            dialogManager.ShowDialog(skillDialogues);
            dialogState = 4;
            // PlayerPrefs.SetInt("DialogueState", 4);
        }
    }

    /// <summary>
    /// Initial delay and first prompt for the movement tutorial step.
    /// Sets state to 1 and allows movement checks to begin.
    /// </summary>
    private IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(0.5f);
        dialogManager.ShowDialog(MovenentDialogues);
        yield return new WaitForSeconds(1f);
        firstTextShown = true;
        dialogState = 1;
        // PlayerPrefs.SetInt("DialogueState", 1);
    }
}
