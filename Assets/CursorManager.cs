using UnityEngine;
/// <summary>
/// Manages the game’s cursor:
/// - Applies a custom texture, hot spot, and cursor mode
/// - Persists across scene loads
/// </summary>
public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    /// <summary>
    /// The texture to use for the cursor.
    /// </summary>
    [SerializeField] private Texture2D cursorTexture;

    /// <summary>
    /// The pixel offset from the texture’s top-left corner
    /// to use as the “click point”.
    /// </summary>
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    /// <summary>
    /// Whether to use hardware (Auto) or software cursor.
    /// </summary>
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

     /// <summary>
    /// Ensures the CursorManager persists and applies the cursor on Awake.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetCustomCursor();
    }

    /// <summary>
    /// Applies the configured custom cursor if a texture is assigned.
    /// </summary>
    private void SetCustomCursor()
    {
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
        else
        {
            Debug.LogWarning("Cursor texture is missing in CursorManager!");
        }
    }
}
