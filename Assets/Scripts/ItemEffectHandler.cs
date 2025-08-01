using System.Collections;
using UnityEngine;

/// <summary>
/// Handles applying an item's effects to the player,
/// including support for temporary buffs.
/// </summary>
public class ItemEffectHandler : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    /// <summary>
    /// Apply the effects of the given item to the player.
    /// </summary>
    public void UseItem(Item item)
    {
        if (item == null || playerStats == null)
            return;

        switch (item.GetItemType())
        {
            case ItemType.Consumable:
                ApplyEffect(item);
                break;

            case ItemType.Tool:
                ApplyEffect(item); // Apply on equip
                // For unequip, call RemoveEffect(item) when needed
                break;

            case ItemType.KeyItem:
                Debug.Log("Used a Key Item: " + item.GetItemName());
                // Handle special interaction logic if needed
                break;
        }
    }

    /// <summary>
    /// Applies stat effect from the item.
    /// Handles temporary and permanent stat boosts.
    /// </summary>
    private void ApplyEffect(Item item)
    {
        AffectedStat stat = item.GetStatToAffect();
        int amount = item.GetAmount();

        if (item.GetIsTemporary())
        {
            StartCoroutine(ApplyTemporaryBuff(stat, amount, item.GetDuration()));
        }
        else
        {
            ModifyStat(stat, amount);
        }
    }

    /// <summary>
    /// Removes stat effect from equipped tool or after temporary buff expires.
    /// </summary>
    public void RemoveEffect(Item item)
    {
        AffectedStat stat = item.GetStatToAffect();
        int amount = item.GetAmount();
        ModifyStat(stat, -amount);
    }

    /// <summary>
    /// Applies a temporary boost for a given duration, then reverts it.
    /// </summary>
    private IEnumerator ApplyTemporaryBuff(AffectedStat stat, int amount, float duration)
    {
        ModifyStat(stat, amount);
        yield return new WaitForSeconds(duration);
        ModifyStat(stat, -amount);
    }

    /// <summary>
    /// Modifies the specified stat on the player.
    /// </summary>
    private void ModifyStat(AffectedStat stat, int amount)
    {
        switch (stat)
        {
            case AffectedStat.Health:
                playerStats.SetCurrentHealth(playerStats.GetCurrentHealth() + amount);
                break;
            case AffectedStat.Mana:
                playerStats.SetCurrentMana(playerStats.GetCurrentMana() + amount);
                break;
            case AffectedStat.Strength:
                playerStats.SetStrength(playerStats.GetStrength() + amount);
                break;
            case AffectedStat.Defense:
                playerStats.SetDefense(playerStats.GetDefense() + amount);
                break;
            case AffectedStat.Speed:
                playerStats.SetSpeed(playerStats.GetSpeed() + amount);
                break;
        }
    }
}
