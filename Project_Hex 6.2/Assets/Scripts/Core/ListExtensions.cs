using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T Draw<T>(this List<T> list)
    {
        if (list.Count == 0) return default;
        int r = 0;
        T t = list[r];
        list.Remove(t);
        return t;
    }
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]); // swap
        }
    }

    public static List<T> TakeTop<T>(this List<T> list, int amount)
    {
        amount = Mathf.Min(amount, list.Count);
        List<T> result = list.GetRange(0, amount);
        list.RemoveRange(0, amount);
        return result;
    }

    public static void PutBottom<T>(this List<T> list, IEnumerable<T> elements)
    {
        list.AddRange(elements);
    }

    public static void PutTop<T>(this List<T> list, IEnumerable<T> elements)
    {
        list.InsertRange(0, elements);
    }
}
