/// <summary>
/// Defines the contract for objects that can be interacted with in the game.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Executes the interaction logic when the player interacts with the object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Determines whether the object can currently be interacted with.
    /// For example, a locked chest might return false until a key is acquired.
    /// </summary>
    /// <returns>True if interaction is allowed; otherwise false.</returns>
    bool CanInteract();
}
