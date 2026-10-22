using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatEntry
{
    public string Key;
    public int Value;
}

[CreateAssetMenu(menuName = "Data/StatObject")]
public class StatRegister : ScriptableObject
{
    [SerializeField]
    public List<StatEntry> DisplayList = new List<StatEntry>();

    public void RegisterStat(string nameToRegister)
    {
        StatEntry entry = DisplayList.Find(
            stat => stat.Key == nameToRegister
        );

        if (entry != null)
        {
            entry.Value++;
        }
        else
        {
            DisplayList.Add(new StatEntry
            {
                Key = nameToRegister,
                Value = 1
            });
        }
    }
}