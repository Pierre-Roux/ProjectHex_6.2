using System.Collections.Generic;
using UnityEngine;

public class CounterSystem : Singleton<CounterSystem>
{
    public void Add(CounterType type, int amount = 1)
    {
        List<Card> cardList = new List<Card>();
        cardList.AddRange(CardSystem.Instance.hand);
        cardList.AddRange(CardSystem.Instance.drawPile);
        cardList.AddRange(CardSystem.Instance.discardPile);
        List<PermanentView> permList = CombatSystem.Instance.Player_Permanents;
        List<EnemySlotView> enemyList = CombatSystem.Instance.Enemy_Permanents;

        foreach (Card card in cardList)
        {
            if (!card.InternCounters.counters.ContainsKey(type))
                card.InternCounters.counters[type] = 0;
            card.InternCounters.counters[type] += amount;            
        }
        foreach (PermanentView perm in permList)
        {
            if (!perm.InternCounters.counters.ContainsKey(type))
                perm.InternCounters.counters[type] = 0;
            perm.InternCounters.counters[type] += amount;
        }
        foreach (EnemySlotView enemy in enemyList)
        {
            if (!enemy.InternCounters.counters.ContainsKey(type))
                enemy.InternCounters.counters[type] = 0;
            enemy.InternCounters.counters[type] += amount;            
        }
    }

    public void Set(CounterType type, int value)
    {
        List<Card> cardList = new List<Card>();
        cardList.AddRange(CardSystem.Instance.hand);
        cardList.AddRange(CardSystem.Instance.drawPile);
        cardList.AddRange(CardSystem.Instance.discardPile);
        List<PermanentView> permList = CombatSystem.Instance.Player_Permanents;
        List<EnemySlotView> enemyList = CombatSystem.Instance.Enemy_Permanents;

        foreach (Card card in cardList)
        {
            card.InternCounters.counters[type] = value;          
        }
        foreach (PermanentView perm in permList)
        {
            perm.InternCounters.counters[type] = value;
        }
        foreach (EnemySlotView enemy in enemyList)
        {
            enemy.InternCounters.counters[type] = value;            
        }
    }

    public void Reset(CounterType type)
    {
        List<Card> cardList = new List<Card>();
        cardList.AddRange(CardSystem.Instance.hand);
        cardList.AddRange(CardSystem.Instance.drawPile);
        cardList.AddRange(CardSystem.Instance.discardPile);
        List<PermanentView> permList = CombatSystem.Instance.Player_Permanents;
        List<EnemySlotView> enemyList = CombatSystem.Instance.Enemy_Permanents;

        foreach (Card card in cardList)
        {
            if (card.InternCounters.counters.ContainsKey(type))
                card.InternCounters.counters[type] = 0;
        }
        foreach (PermanentView perm in permList)
        {
            if (perm.InternCounters.counters.ContainsKey(type))
                perm.InternCounters.counters[type] = 0;
        }
        foreach (EnemySlotView enemy in enemyList)
        {
            if (enemy.InternCounters.counters.ContainsKey(type))
                enemy.InternCounters.counters[type] = 0;
        }
    }
}

[System.Serializable]
public class CounterManager
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