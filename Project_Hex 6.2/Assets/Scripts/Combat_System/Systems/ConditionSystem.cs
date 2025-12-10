using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionSystem : Singleton<ConditionSystem>
{
    public bool TestCondition(List<DynamicConditionInfo> DynamicConditionInfos, Card TestCard = null, PermanentView TestpermanentView = null, EnemySlotView TestenemySlotView = null,  Card TriggerCard = null, PermanentView TriggerpermanentView = null, EnemySlotView TriggerenemySlotView = null)
    {
        foreach (DynamicConditionInfo Condition in DynamicConditionInfos)
        {
            int Amount = 0;
            bool ConditionResult;
            CounterManager counterManager = new();

            if (Condition.DynamicCondition != DynamicCondition.NULL)
            {
                switch (Condition.DynamicCondition)
                {
                    case DynamicCondition.NoCardsInHands:
                        if (CardSystem.Instance.hand.Count == 0)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.isHollow:
                        if (TestpermanentView != null)
                        {
                            var HollowKeyword = TestpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
                            if (HollowKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else if (TestenemySlotView != null)
                        {
                            var HollowKeyword = TestenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
                            if (HollowKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;



                    case DynamicCondition.isDecay:
                        if (TestpermanentView != null)
                        {
                            var decayKeyword = TestpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Decay);
                            if (decayKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else if (TestenemySlotView != null)
                        {
                            var decayKeyword = TestenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Decay);
                            if (decayKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;


                    case DynamicCondition.isInvoc:
                        if (TestpermanentView != null)
                        {
                            var InvocKeyword = TestpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
                            if (InvocKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else if (TestenemySlotView != null)
                        {
                            var InvocKeyword = TestenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
                            if (InvocKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;


                    case DynamicCondition.isArtillery:
                        if (TestpermanentView != null)
                        {
                            var ArtilleryKeyword = TestpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Artillery);
                            if (ArtilleryKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else if (TestenemySlotView != null)
                        {
                            var ArtilleryKeyword = TestpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Artillery);
                            if (ArtilleryKeyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifYouControlHollow:
                        bool trueConditionFound1 = false;
                        foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                        {
                            var HollowKeyword = item.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
                            if (HollowKeyword != null)
                            {
                                trueConditionFound1 = true;
                            }
                        }
                        if (trueConditionFound1)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;


                    case DynamicCondition.ifYouControlDecay:
                        bool trueConditionFound2 = false;
                        foreach (PermanentView perm in CombatSystem.Instance.Player_Permanents)
                        {
                            var decayKeyword = TestenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Decay);
                            if (decayKeyword != null)
                            {
                                trueConditionFound2 = true;
                            }
                        }
                        if (trueConditionFound2)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifYouControlInvoc:
                        bool trueConditionFound3 = false;
                        foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                        {
                            var InvocKeyword = item.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
                            if (InvocKeyword != null)
                            {
                                trueConditionFound3 = true;
                            }
                        }
                        if (trueConditionFound3)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifYouControlArtillery:
                        bool trueConditionFound4 = false;
                        foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                        {
                            var ArtilleryKeyword = item.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Artillery);
                            if (ArtilleryKeyword != null)
                            {
                                trueConditionFound4 = true;
                            }
                        }
                        if (trueConditionFound4)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifEventPermanentIsTypeOfTestType:
                        if (TriggerpermanentView != null)
                        {
                            var Keyword = TriggerpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                            if (Keyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else if (TriggerenemySlotView != null)
                        {
                            var Keyword = TriggerenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                            if (Keyword != null)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifEventPermanentIsPlayer:
                        if (TriggerpermanentView != null)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifEventPermanentIsEnemy:
                        if (TriggerenemySlotView != null)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifEventPermanentIsVessel:
                        if (TriggerpermanentView != null)
                        {
                            var InvocKeyword = TriggerpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
                            if (InvocKeyword != null)
                            {
                                ConditionResult = false;
                            }
                            else
                            {
                                ConditionResult = true;
                            }
                        }
                        else if (TriggerenemySlotView != null)
                        {
                            var InvocKeyword = TriggerenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
                            if (InvocKeyword != null)
                            {
                                ConditionResult = false;
                            }
                            else
                            {
                                ConditionResult = true;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifEventCardTriggerIsVessel:
                        if (TriggerCard != null)
                        {
                            if (!TriggerCard.IsSpell)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifEventCardTriggerIsSpell:
                        if (TriggerenemySlotView != null)
                        {
                            if (TriggerCard.IsSpell)
                            {
                                ConditionResult = true;
                            }
                            else
                            {
                                ConditionResult = false;
                            }
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifGlobalCounterOfTypeSupToValue:
                        Amount = CombatSystem.Instance.GlobalCounters.Get(Condition.CounterType);
                        if (Amount > Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifGlobalCounterOfTypeInfToValue:
                        Amount = CombatSystem.Instance.GlobalCounters.Get(Condition.CounterType);
                        if (Amount < Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifInternCounterOfTypeSupToValue:
                        if (TestpermanentView != null)
                        {
                            counterManager = TestpermanentView.InternCounters;
                            Amount = counterManager.Get(Condition.CounterType);
                        }
                        else if (TestenemySlotView != null)
                        {
                            counterManager = TestenemySlotView.InternCounters;
                            Amount = counterManager.Get(Condition.CounterType);
                        }
                        else if (TestCard != null)
                        {
                            counterManager = TestCard.InternCounters;
                            Amount = counterManager.Get(Condition.CounterType);
                        }
                        else
                        {
                            Amount = -1000;
                        }

                        if (Amount > Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifInternCounterOfTypeInfToValue:
                        if (TestpermanentView != null)
                        {
                            counterManager = TestpermanentView.InternCounters;
                            Amount = counterManager.Get(Condition.CounterType);
                        }
                        else if (TestenemySlotView != null)
                        {
                            counterManager = TestenemySlotView.InternCounters;
                            Amount = counterManager.Get(Condition.CounterType);
                        }
                        else if (TestCard != null)
                        {
                            counterManager = TestCard.InternCounters;
                            Amount = counterManager.Get(Condition.CounterType);
                        }
                        else
                        {
                            Amount = 1000;
                        }

                        Debug.Log("Test : " + Amount + " < " + Condition.TestValue);

                        if (Amount < Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.DynamicAmountSupOrEqualsToValue:
                        Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmount);
                        if (Amount >= Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;


                    case DynamicCondition.DynamicAmountInfOrEqualsToValue:
                        Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmount);
                        if (Amount <= Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;


                    case DynamicCondition.DynamicAmountSupToValue:
                        Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmount);
                        if (Amount > Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;


                    case DynamicCondition.DynamicAmountInfToValue:
                        Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmount);
                        if (Amount < Condition.TestValue)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    default:
                        ConditionResult = false;
                        break;
                }
            }
            else
            {
                ConditionResult = true;
            }

            if (ConditionResult == false)
            {
                return false;
            }
        }

        return true;
    }
}
