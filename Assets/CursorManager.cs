using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetCustomCursor();
    }

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
