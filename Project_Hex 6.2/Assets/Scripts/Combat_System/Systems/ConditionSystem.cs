using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSystem : Singleton<ConditionSystem>
{
    public bool TestCondition(List<DynamicConditionInfo> DynamicConditionInfos, Card TestCard = null, PermanentView TestpermanentView = null, EnemySlotView TestenemySlotView = null)
    {
        foreach (DynamicConditionInfo Condition in DynamicConditionInfos)
        {
            int Amount;
            bool ConditionResult;
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
                            if (TestpermanentView.permaTypes.Contains(PermaTypes.Hollow))
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
                            if (TestenemySlotView.permaTypes.Contains(PermaTypes.Hollow))
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
                            if (TestpermanentView.permaTypes.Contains(PermaTypes.Decay))
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
                            if (TestenemySlotView.permaTypes.Contains(PermaTypes.Decay))
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
                            if (TestpermanentView.permaTypes.Contains(PermaTypes.Invoc))
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
                            if (TestenemySlotView.permaTypes.Contains(PermaTypes.Invoc))
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
                            if (TestpermanentView.permaTypes.Contains(PermaTypes.Artillery))
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
                            if (TestenemySlotView.permaTypes.Contains(PermaTypes.Artillery))
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
                            if (item.permaTypes.Contains(PermaTypes.Hollow))
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
                        foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                        {
                            if (item.permaTypes.Contains(PermaTypes.Decay))
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
                            if (item.permaTypes.Contains(PermaTypes.Invoc))
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
                            if (item.permaTypes.Contains(PermaTypes.Artillery))
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
                        if (TestpermanentView != null)
                        {
                            if (TestpermanentView.permaTypes.Contains(Condition.TestType))
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
                            if (TestenemySlotView.permaTypes.Contains(Condition.TestType))
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
                        if (TestpermanentView != null)
                        {
                            ConditionResult = true;
                        }
                        else
                        {
                            ConditionResult = false;
                        }
                        break;

                    case DynamicCondition.ifEventPermanentIsEnemy:
                        if (TestenemySlotView != null)
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
