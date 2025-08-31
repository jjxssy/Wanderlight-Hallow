using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public static class StatisticsManager
{
    private const string StatPrefix = "stat_";
    private const string MasterListKey = "STAT_MASTER_LIST";
    private static List<string> _statKeys;

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


    public static int Get(string statName)
    {
        return PlayerPrefs.GetInt(StatPrefix + statName, 0);
    }

    public static void Set(string statName, int value)
    {
        RegisterStat(statName); 
        PlayerPrefs.SetInt(StatPrefix + statName, value);
    }


    public static void Increase(string statName, int amount = 1)
    {
        RegisterStat(statName);
        int currentValue = Get(statName);
        PlayerPrefs.SetInt(StatPrefix + statName, currentValue + amount);
    }

    public static void Reset(string statName)
    {
        PlayerPrefs.SetInt(StatPrefix + statName, 0);
    }

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
    public static void Save()
    {
        PlayerPrefs.Save();
    }
}