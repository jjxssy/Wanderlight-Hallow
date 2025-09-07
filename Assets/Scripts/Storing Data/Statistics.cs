using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Static manager for tracking player statistics (kills, coins, achievements, etc.).
/// Stores values in <see cref="PlayerPrefs"/> with persistence between sessions.
/// </summary>
public static class StatisticsManager
{
    /// <summary>
    /// Prefix applied to all stat keys stored in <see cref="PlayerPrefs"/>.
    /// </summary>
    private const string StatPrefix = "stat_";

    /// <summary>
    /// Key used to store the master list of all registered statistics.
    /// </summary>
    private const string MasterListKey = "STAT_MASTER_LIST";

    /// <summary>
    /// Cached list of registered stat keys.
    /// </summary>
    private static List<string> _statKeys;

    /// <summary>
    /// Loads the list of registered stat keys from <see cref="PlayerPrefs"/> if not already cached.
    /// </summary>
    private static void LoadStatKeys()
    {
        if (_statKeys == null)
        {
            string rawList = PlayerPrefs.GetString(MasterListKey, "");
            if (string.IsNullOrEmpty(rawList))
            {
                _statKeys = new List<string>();
            }
            else
            {
                _statKeys = rawList.Split(',').ToList();
            }
        }
    }

    /// <summary>
    /// Ensures the given stat is registered in the master list.
    /// </summary>
    /// <param name="statName">The name of the stat to register.</param>
    private static void RegisterStat(string statName)
    {
        LoadStatKeys();
        if (!_statKeys.Contains(statName))
        {
            _statKeys.Add(statName);
            string rawList = string.Join(",", _statKeys);
            PlayerPrefs.SetString(MasterListKey, rawList);
        }
    }

    /// <summary>
    /// Gets the current value of a statistic.
    /// </summary>
    /// <param name="statName">The name of the statistic.</param>
    /// <returns>The current value, or 0 if not found.</returns>
    public static int Get(string statName)
    {
        return PlayerPrefs.GetInt(StatPrefix + statName, 0);
    }

    /// <summary>
    /// Sets a statistic to a specific value.
    /// </summary>
    /// <param name="statName">The name of the statistic.</param>
    /// <param name="value">The new value to assign.</param>
    public static void Set(string statName, int value)
    {
        RegisterStat(statName); 
        PlayerPrefs.SetInt(StatPrefix + statName, value);
    }

    /// <summary>
    /// Increases a statistic by the given amount (default = 1).
    /// </summary>
    /// <param name="statName">The name of the statistic.</param>
    /// <param name="amount">The amount to increase (default 1).</param>
    public static void Increase(string statName, int amount = 1)
    {
        RegisterStat(statName);
        int currentValue = Get(statName);
        PlayerPrefs.SetInt(StatPrefix + statName, currentValue + amount);
    }

    /// <summary>
    /// Resets a statistic’s value back to 0.
    /// </summary>
    /// <param name="statName">The name of the statistic.</param>
    public static void Reset(string statName)
    {
        PlayerPrefs.SetInt(StatPrefix + statName, 0);
    }

    /// <summary>
    /// Clears all statistics and the master list from <see cref="PlayerPrefs"/>.
    /// </summary>
    public static void ClearAllStats()
    {
        LoadStatKeys();
        foreach (string key in _statKeys)
        {
            PlayerPrefs.DeleteKey(StatPrefix + key);
        }

        PlayerPrefs.DeleteKey(MasterListKey);
        _statKeys.Clear();

        Debug.Log("All game statistics have been cleared.");
    }

    /// <summary>
    /// Forces <see cref="PlayerPrefs"/> to save all current statistics to disk.
    /// </summary>
    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
