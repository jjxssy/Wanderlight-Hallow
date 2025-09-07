using UnityEngine;

/// <summary>
/// Provides global helper methods that can be used anywhere in the project.
/// </summary>
public static class GlobalHelper
{
    /// <summary>
    /// Generates a simple unique ID for a <see cref="GameObject"/>
    /// based on its scene name and world position.
    /// </summary>
    /// <param name="obj">The target GameObject.</param>
    /// <returns>
    /// A string in the format: <c>{sceneName}_{posX}_{posY}</c>.
    /// Returns <c>NullObj</c> if the object is null.
    /// </returns>
    public static string GenerateUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}";
    }
}
