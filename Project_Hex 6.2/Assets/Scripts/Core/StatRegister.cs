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
    public List<string> Stats = new List<string>();

    [SerializeField]
    public List<StatEntry> DisplayList = new List<StatEntry>();

    public void RegisterStat(string nameToRegister)
    {
        Stats.Add(nameToRegister);
    }

    public void ViewStats()
    {
        foreach (string item in Stats)
        {
            StatEntry entry = DisplayList.Find(
                stat => stat.Key == item
            );

            if (entry != null)
            {
                entry.Value++;
            }
            else
            {
                DisplayList.Add(new StatEntry
                {
                    Key = item,
                    Value = 1
                });
            }
        }

        string displayMsg =
            "----------- Stat Globales de draft -----------\n";

        foreach (StatEntry entry in DisplayList)
        {
            displayMsg += $"{entry.Key} : {entry.Value}\n";
        }

        Debug.Log(displayMsg);
    }
}