using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CounterModel
{
    [SerializeField] public Dictionary<CounterTypeInfo, int> counters = new();

    public void Add(CounterTypeInfo typeInfo, int amount = 1)
    {
        if (!counters.ContainsKey(typeInfo))
            counters[typeInfo] = 0;
        counters[typeInfo] += amount;
    }

    public int Get(CounterTypeInfo typeInfo)
    {
        return counters.TryGetValue(typeInfo, out int value) ? value : 0;
    }

    public void Set(CounterTypeInfo typeInfo, int value)
    {
        counters[typeInfo] = value;
    }

    public void Reset(CounterTypeInfo typeInfo)
    {
        if (counters.ContainsKey(typeInfo))
            counters[typeInfo] = 0;
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
