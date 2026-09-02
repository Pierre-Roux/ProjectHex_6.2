using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player")]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    [field: Header("Mandatory")]
    [field: SerializeField] public string Name;
    [field: SerializeField] public CardPool deckData;
    [field: SerializeField] public NavCardPool navDeckData;

    [field: Header("Player Core")]
    [field: SerializeField] public CardData Core;
}
