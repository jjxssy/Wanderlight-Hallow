/// <summary>
/// Defines the contract for any object that can take damage.
/// Commonly implemented by enemies, players, or destructible objects.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Applies damage to the object.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    void TakeDamage(int damage);
}