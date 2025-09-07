/// <summary>
/// Defines an interactable object in the game world.
/// Any component implementing this interface can be interacted with,
/// and should provide rules for when interaction is allowed.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Performs the interaction logic.
    /// Called when the player successfully interacts with the object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Determines whether the object can currently be interacted with.
    /// For example, a locked chest may return false until unlocked.
    /// </summary>
    /// <returns>True if interaction is possible; otherwise false.</returns>
    bool CanInteract();
}
