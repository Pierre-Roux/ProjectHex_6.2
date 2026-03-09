using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CounterModel
{
    [SerializeField] 
    public Dictionary<CounterType, int> counters = new();

    public void Add(CounterType type, int amount = 1)
    {
        if (!counters.ContainsKey(type))
            counters[type] = 0;
        counters[type] += amount;
    }

    public int Get(CounterType type)
    {
        return counters.TryGetValue(type, out int value) ? value : 0;
    }

    public void Set(CounterType type, int value)
    {
        counters[type] = value;
    }

    public void Reset(CounterType type)
    {
        if (counters.ContainsKey(type))
            counters[type] = 0;
    }

    public void ClearAll()
    {
        counters.Clear();
    }

    public void LogAll(string ownerName = "Unknown")
    {
        Debug.Log($"--- Counters for {ownerName} ---");
        foreach (var kvp in counters)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }
}
