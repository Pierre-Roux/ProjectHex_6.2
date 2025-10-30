using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player")]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    [field: Header("Mandatory")]
    [field: SerializeField] public String Name;
    [field: SerializeField] public List<CardData> deckData;


    [field: Header("Player Core")]
    [field: SerializeField] public int CoreHealth;
    [field: SerializeField] public Sprite CoreImage;
    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference BeingDamageSound;
    [SerializeField] public EventReference BeingHealSound;
    [SerializeField] public EventReference BeingShieldSound;
    [SerializeField] public EventReference LoseShieldSound;
    [SerializeField] public EventReference TakeLifeLossSound;
    [SerializeField] public EventReference BuffLifeSound;
    [SerializeField] public EventReference DebuffLifeSound;
    [SerializeField] public EventReference SelectedSound;
    [SerializeField] public EventReference UnSelectedSound;
}
