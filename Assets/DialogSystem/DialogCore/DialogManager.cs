using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// Simple dialogue controller:
/// 1) Shows a panel and types out each line character-by-character.
/// 2) Holding the advance key types faster.
/// 3) After a line finishes typing, waits until the advance key is pressed to continue.
/// 4) Exposes Start/Stop and an IsDialogueActive query.
/// </summary>
public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private bool isDialogueActive = false;
    private bool isFastDialogueActive = false;
    private Coroutine activeCoroutine;
    private string[] currentDialogue;

    /// <summary>
    /// Initializes the panel as hidden.
    /// </summary>
    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Tracks whether the advance key is held to speed up typing.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) isFastDialogueActive = true;
        if (Input.GetKeyUp(KeyCode.E)) isFastDialogueActive = false;
    }

    /// <summary>
    /// Starts showing the given dialogue lines, one after another with typewriter effect.
    /// </summary>
    /// <param name="dialog">Array of lines to display in order.</param>
    public void ShowDialog(string[] dialog)
    {
        StopDialog();
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        currentDialogue = dialog;
        activeCoroutine = StartCoroutine(DialogCouroutine());
    }

    /// <summary>
    /// Immediately stops any active dialogue and hides the panel.
    /// </summary>
    public void StopDialog()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            isDialogueActive = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        }
    }

    /// <summary>
    /// Typewriter flow: types each line, then waits for the advance key to proceed.
    /// </summary>
    private IEnumerator DialogCouroutine()
    {
        yield return null;
        if (currentDialogue != null)
        {
            foreach (string sentence in currentDialogue)
            {
                dialogueText.text = "";
                // Type out characters with a dynamic delay
                foreach (char letter in sentence.ToCharArray())
                {
                    dialogueText.text += letter;
                    if (isFastDialogueActive)
                        yield return new WaitForSeconds(0.005f);
                    else
                        yield return new WaitForSeconds(0.05f);
                }
                yield return null;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                yield return null;
            }
        }
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
    }
    
    /// <summary>
    /// Returns true if the dialogue system is currently showing lines.
    /// </summary>
    public bool IsDialogueActive() { return isDialogueActive; }
}
