using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SerializeReferenceEditor;

public class EnemyView : MonoBehaviour
{
    [field: SerializeField] public int Tier;
    [SerializeField] public List<EnemyPermanentData> EnemyPreset;
    [SerializeField] public EnemyZoneView WeaponZone;
    [SerializeField] public EnemyZoneView ShieldZone;
    [SerializeField] public EnemyZoneView SupportZone;
    [SerializeField] public EnemySlotView CoreSlot;
    [HideInInspector] public List<GameAction> SetupActions = new();

    [field : SerializeReference, SR] public List<IntentConstruct> IntentConstructs { get; private set; }
    [SerializeField] public List<string> ConstructSequence = new();
    [SerializeField] public bool LoopingSequence;
    [HideInInspector] public int sequenceIndex = 0;

    public void Setup()
    {
        CoreSlot.setup();
        CombatSystem.Instance.Enemy_Permanents.Add(CoreSlot);

        foreach (EnemyPermanentData enemy in EnemyPreset)
        {
            EnemySlotViewCreator.Instance.CreateEnemySlotViewCreator(enemy, enemy.permanentArea, true, this);
        }
    }
}
