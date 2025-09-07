using UnityEngine;
/// <summary>
/// Global manager for applying a custom cursor texture.
/// - Sets the cursor on Awake
/// - Persists across scene loads
/// </summary>
public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    /// <summary>
    /// Ensures this object persists across scenes and applies the cursor.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetCustomCursor();
    }
    
    /// <summary>
    /// Applies the custom cursor if a texture is assigned.
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
