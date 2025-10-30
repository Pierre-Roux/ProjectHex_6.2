using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSystem : Singleton<ConditionSystem>
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<TestConditionGA>(TestChoiceConditionPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<TestConditionGA>();
    }
    public IEnumerator TestChoiceConditionPerformer(TestConditionGA testConditionGA)
    {
        int Amount = 0;
        if (testConditionGA.DynamicCondition != DynamicCondition.NULL)
        {
            switch (testConditionGA.DynamicCondition)
            {
                case DynamicCondition.NoCardsInHands:
                    if (CardSystem.Instance.hand.Count == 0)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                case DynamicCondition.isHollow:
                    if (testConditionGA.TestPermanentView.permaTypes.Contains(PermaTypes.Hollow))
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                case DynamicCondition.isDecay:
                    if (testConditionGA.TestPermanentView.permaTypes.Contains(PermaTypes.Decay))
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                case DynamicCondition.isInvoc:
                    if (testConditionGA.TestPermanentView.permaTypes.Contains(PermaTypes.Invoc))
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                case DynamicCondition.isArtillery:
                    if (testConditionGA.TestPermanentView.permaTypes.Contains(PermaTypes.Artillery))
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                case DynamicCondition.ifYouControlHollow:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Hollow))
                        {
                            ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                            break;
                        }
                    }
                    ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    break;
                case DynamicCondition.ifYouControlDecay:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Decay))
                        {
                            ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                            break;
                        }
                    }
                    ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    break;
                case DynamicCondition.ifYouControlInvoc:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Invoc))
                        {
                            ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                            break;
                        }
                    }
                    ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    break;
                case DynamicCondition.ifYouControlArtillery:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Artillery))
                        {
                            ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                            break;
                        }
                    }
                    ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    break;
                case DynamicCondition.ValueSupToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(testConditionGA.TestDynamicAmount);
                    if (testConditionGA.Value > Amount)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                case DynamicCondition.ValueInfToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(testConditionGA.TestDynamicAmount);
                    if (testConditionGA.Value < Amount)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;

                case DynamicCondition.ValueSupOrEqualsToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(testConditionGA.TestDynamicAmount);
                    if (testConditionGA.Value >= Amount)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;

                case DynamicCondition.ValueInfOrEqualsToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(testConditionGA.TestDynamicAmount);
                    if (testConditionGA.Value <= Amount)
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnTrue.GetGameAction());
                    }
                    else
                    {
                        ActionSystem.Instance.AddReaction(testConditionGA.EffectOnFalse.GetGameAction());
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {

        }
        yield return null;
    }

    public bool TestCondition(DynamicCondition TestDynamicCondition, DynamicAmount TestDynamicAmount, int TestValue, Card TestCard = null, PermanentView TestpermanentView = null, EnemySlotView TestenemySlotView = null, PermaTypes TestType = PermaTypes.NULL)
    {
        int Amount = 0;
        if (TestDynamicCondition != DynamicCondition.NULL)
        {
            switch (TestDynamicCondition)
            {
                case DynamicCondition.NoCardsInHands:
                    if (CardSystem.Instance.hand.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case DynamicCondition.isHollow:
                    if (TestpermanentView != null)
                    {
                        if (TestpermanentView.permaTypes.Contains(PermaTypes.Hollow))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (TestenemySlotView != null)
                    {
                        if (TestenemySlotView.permaTypes.Contains(PermaTypes.Hollow))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }



                case DynamicCondition.isDecay:
                    if (TestpermanentView != null)
                    {
                        if (TestpermanentView.permaTypes.Contains(PermaTypes.Decay))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (TestenemySlotView != null)
                    {
                        if (TestenemySlotView.permaTypes.Contains(PermaTypes.Decay))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }


                case DynamicCondition.isInvoc:
                    if (TestpermanentView != null)
                    {
                        if (TestpermanentView.permaTypes.Contains(PermaTypes.Invoc))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (TestenemySlotView != null)
                    {
                        if (TestenemySlotView.permaTypes.Contains(PermaTypes.Invoc))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }


                case DynamicCondition.isArtillery:
                    if (TestpermanentView != null)
                    {
                        if (TestpermanentView.permaTypes.Contains(PermaTypes.Artillery))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (TestenemySlotView != null)
                    {
                        if (TestenemySlotView.permaTypes.Contains(PermaTypes.Artillery))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                case DynamicCondition.ifYouControlHollow:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Hollow))
                        {
                            return true;
                        }
                    }
                    return false;


                case DynamicCondition.ifYouControlDecay:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Decay))
                        {
                            return true;

                        }
                    }
                    return false;

                case DynamicCondition.ifYouControlInvoc:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Invoc))
                        {
                            return true;
                        }
                    }
                    return false;

                case DynamicCondition.ifYouControlArtillery:
                    foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                    {
                        if (item.permaTypes.Contains(PermaTypes.Artillery))
                        {
                            return true;
                        }
                    }
                    return false;

                case DynamicCondition.ifEventPermanentIsTypeOfTestType:
                    if (TestpermanentView != null)
                    {
                        if (TestpermanentView.permaTypes.Contains(TestType))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (TestenemySlotView != null)
                    {
                        if (TestenemySlotView.permaTypes.Contains(TestType))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                case DynamicCondition.ifEventPermanentIsPlayer:
                    if (TestpermanentView != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case DynamicCondition.ifEventPermanentIsEnemy:
                    if (TestenemySlotView != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case DynamicCondition.ValueSupToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(TestDynamicAmount);
                    if (TestValue > Amount)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case DynamicCondition.ValueInfToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(TestDynamicAmount);
                    if (TestValue < Amount)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case DynamicCondition.ValueSupOrEqualsToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(TestDynamicAmount);
                    if (TestValue >= Amount)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case DynamicCondition.ValueInfOrEqualsToDynamicAmount:
                    Amount = TargetSystem.Instance.GetDynamicAmount(TestDynamicAmount);
                    if (TestValue <= Amount)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }
}
