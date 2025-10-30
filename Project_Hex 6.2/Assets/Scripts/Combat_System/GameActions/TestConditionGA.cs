using JetBrains.Annotations;
using UnityEngine;

public class TestConditionGA : GameAction
{
    public DynamicCondition DynamicCondition;
    public Effect EffectOnTrue;
    public Effect EffectOnFalse;
    public int Value;
    public DynamicAmount TestDynamicAmount;
    public CardView TestCardview;
    public PermanentView TestPermanentView;
    public EnemySlotView TestEnemySlotView;
    public TestConditionGA(DynamicCondition dynamicCondition, Effect effectOnTrue, Effect effectOnFalse, int value = 0, DynamicAmount dynamicAmount = DynamicAmount.NULL, CardView cardView = null, PermanentView permanentView = null, EnemySlotView enemySlotView = null)
    {
        DynamicCondition = dynamicCondition;
        EffectOnTrue = effectOnTrue;
        EffectOnFalse = effectOnFalse;
        Value = value;
        TestDynamicAmount = dynamicAmount;
        TestCardview = cardView;
        TestPermanentView = permanentView;
        TestEnemySlotView = enemySlotView;
    }
}
