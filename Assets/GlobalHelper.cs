using UnityEngine;
/// <summary>
/// Provides utility methods that can be accessed globally across the project.
/// </summary>
public static class GlobalHelper
{   /// <summary>
    /// Generates a simple unique ID for a GameObject based on its scene name and position.
    /// </summary>
    /// <param name="obj">The target GameObject.</param>
    /// <returns>
    /// A string in the format: "{sceneName}_{posX}_{posY}".  
    /// Returns "NullObj" if the GameObject is null.
    /// </returns>
    public static string GenerateUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}";
    }
}
