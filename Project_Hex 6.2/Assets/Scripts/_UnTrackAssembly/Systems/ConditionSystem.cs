using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionSystem : Singleton<ConditionSystem>
{
    public bool TestCondition(List<DynamicConditionInfo> DynamicConditionInfos, Card TestCard = null, PermanentView TestpermanentView = null, EnemySlotView TestenemySlotView = null,  Card TriggerCard = null, PermanentView TriggerpermanentView = null, EnemySlotView TriggerenemySlotView = null, bool AtResolve = false, Card TargetCard = null, PermanentView TargetPermanentView = null, EnemySlotView TargetEnemySlotView = null, GameObject Actionner = null)
    {
        if (DynamicConditionInfos == null) return true;
        foreach (DynamicConditionInfo Condition in DynamicConditionInfos)
        {
            int Amount = 0;
            bool ConditionResult = true;
            CounterModel counterManager = new();

            if (!AtResolve)
            {
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

                        case DynamicCondition.CardsInHands:
                            if (CardSystem.Instance.hand.Count == 0)
                            {
                                ConditionResult = false;
                            }
                            else
                            {
                                ConditionResult = true;
                            }
                            break;

                        case DynamicCondition.ifPermanentIsTypeOfTestType:
                            if (TestpermanentView != null)
                            {
                                var Keyword = TestpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
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
                                var Keyword = TestenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
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

                        case DynamicCondition.ifPermanentIsNotTypeOfTestType:
                            if (TestpermanentView != null)
                            {
                                var Keyword = TestpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
                                {
                                    ConditionResult = false;
                                }
                                else
                                {
                                    ConditionResult = true;
                                }
                            }
                            else if (TestenemySlotView != null)
                            {
                                var Keyword = TestenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
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

                        case DynamicCondition.ifYouControlTypeOfTestType:
                            bool trueConditionFound1 = false;
                            foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                            {
                                var Keyword = item.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
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

                        case DynamicCondition.ifEventPermanentIsNotTypeOfTestType:
                            if (TriggerpermanentView != null)
                            {
                                var Keyword = TriggerpermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
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
                                var Keyword = TriggerenemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
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

                        case DynamicCondition.ifGlobalCounterOfTypeSupToValue:
                            Amount = CombatSystem.Instance.GlobalCounters.Get(Condition.CounterTypeInfo);
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
                            Amount = CombatSystem.Instance.GlobalCounters.Get(Condition.CounterTypeInfo);
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
                                Amount = counterManager.Get(Condition.CounterTypeInfo);
                            }
                            else if (TestenemySlotView != null)
                            {
                                counterManager = TestenemySlotView.InternCounters;
                                Amount = counterManager.Get(Condition.CounterTypeInfo);
                            }
                            else if (TestCard != null)
                            {
                                counterManager = TestCard.InternCounters;
                                Amount = counterManager.Get(Condition.CounterTypeInfo);
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
                                Amount = counterManager.Get(Condition.CounterTypeInfo);
                            }
                            else if (TestenemySlotView != null)
                            {
                                counterManager = TestenemySlotView.InternCounters;
                                Amount = counterManager.Get(Condition.CounterTypeInfo);
                            }
                            else if (TestCard != null)
                            {
                                counterManager = TestCard.InternCounters;
                                Amount = counterManager.Get(Condition.CounterTypeInfo);
                            }
                            else
                            {
                                Amount = 1000;
                            }

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
                            Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmountInfo);
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
                            Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmountInfo);
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
                            Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmountInfo);
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
                            Amount = TargetSystem.Instance.GetDynamicAmount(Condition.TestDynamicAmountInfo);
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
                            ConditionResult = true;
                            break;
                    }
                }
                else
                {
                    ConditionResult = true;
                }
            }
            else
            {
                if (Condition.DynamicCondition != DynamicCondition.NULL)
                {
                    switch (Condition.DynamicCondition)
                    {
                        case DynamicCondition.ifTargetIsTypeOfTestType:
                            if (TargetPermanentView != null)
                            {
                                var Keyword = TargetPermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
                                {
                                    ConditionResult = true;
                                }
                                else
                                {
                                    ConditionResult = false;
                                }
                            }
                            else if (TargetEnemySlotView != null)
                            {
                                var Keyword = TargetEnemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
                                {
                                    ConditionResult = true;
                                }
                                else
                                {
                                    ConditionResult = false;
                                }
                            }
                            else if (TargetCard != null)
                            {
                                var Keyword = TargetCard.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
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

                        case DynamicCondition.ifTargetIsNotTypeOfTestType:
                            if (TargetPermanentView != null)
                            {
                                var Keyword = TargetPermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
                                {
                                    ConditionResult = false;
                                }
                                else
                                {
                                    ConditionResult = true;
                                }
                            }
                            else if (TargetEnemySlotView != null)
                            {
                                var Keyword = TargetEnemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
                                {
                                    ConditionResult = false;
                                }
                                else
                                {
                                    ConditionResult = true;
                                }
                            }
                            else if (TargetCard != null)
                            {
                                var Keyword = TargetCard.KeyWords.FirstOrDefault(k => k.keyWordType == Condition.TestType);
                                if (Keyword != null)
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

                        case DynamicCondition.ifTargetParamIsSupToValue:
                            switch (Condition.basicParam)
                            {
                                case BasicParam.Life:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentLife;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Life;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentLife;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Life;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var Keyword = TargetCard.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.PermaCard);
                                        if (Keyword != null)
                                        {
                                            var TestedParam = 0;
                                            if (Condition.currentParam)
                                            {
                                                TestedParam = TargetCard.Life;
                                            }
                                            else
                                            {
                                                TestedParam = TargetCard.Life;
                                            }
                                            if (TestedParam > Condition.TestValue)
                                            {
                                                ConditionResult = true;
                                            }
                                            else
                                            {
                                                ConditionResult = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ConditionResult = false;
                                    }
                                    break;
                                case BasicParam.Armor:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentArmor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Armor;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentArmor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Armor;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        // ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Armor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.Armor;
                                        }
                                        if (TestedParam > Condition.TestValue)
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
                                case BasicParam.Cost:
                                    if (TargetPermanentView != null)
                                    {
                                        // Ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.cost;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.cost;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.RefCardView.CurrentCost;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.cost;
                                        }
                                        if (TestedParam > Condition.TestValue)
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
                                case BasicParam.Power:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentPower;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Armor;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentPower;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Armor;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        // ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Power;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.Power;
                                        }
                                        if (TestedParam > Condition.TestValue)
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
                                case BasicParam.Durability:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.Durability;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.MaxDurability;
                                        }
                                        if (TestedParam > Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Durability;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.MaxDurability;
                                        }
                                        if (TestedParam > Condition.TestValue)
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
                                default:
                                    ConditionResult = false;
                                    break;
                            }
                            break;

                        case DynamicCondition.ifTargetParamIsInfToValue:
                            switch (Condition.basicParam)
                            {
                                case BasicParam.Life:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentLife;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Life;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentLife;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Life;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var Keyword = TargetCard.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.PermaCard);
                                        if (Keyword != null)
                                        {
                                            var TestedParam = 0;
                                            if (Condition.currentParam)
                                            {
                                                TestedParam = TargetCard.Life;
                                            }
                                            else
                                            {
                                                TestedParam = TargetCard.Life;
                                            }
                                            if (TestedParam < Condition.TestValue)
                                            {
                                                ConditionResult = true;
                                            }
                                            else
                                            {
                                                ConditionResult = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ConditionResult = false;
                                    }
                                    break;
                                case BasicParam.Armor:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentArmor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Armor;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentArmor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Armor;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        // ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Armor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.Armor;
                                        }
                                        if (TestedParam < Condition.TestValue)
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
                                case BasicParam.Cost:
                                    if (TargetPermanentView != null)
                                    {
                                        // Ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.cost;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.cost;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.RefCardView.CurrentCost;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.cost;
                                        }
                                        if (TestedParam < Condition.TestValue)
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
                                case BasicParam.Power:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentPower;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Power;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentPower;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Power;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        // ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Power;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.Power;
                                        }
                                        if (TestedParam < Condition.TestValue)
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
                                case BasicParam.Durability:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.Durability;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.MaxDurability;
                                        }
                                        if (TestedParam < Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Durability;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.MaxDurability;
                                        }
                                        if (TestedParam < Condition.TestValue)
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
                                default:
                                    ConditionResult = false;
                                    break;
                            }
                            break;

                        case DynamicCondition.ifTargetParamIsEqualToValue:
                            switch (Condition.basicParam)
                            {
                                case BasicParam.Life:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentLife;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Life;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentLife;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Life;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var Keyword = TargetCard.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.PermaCard);
                                        if (Keyword != null)
                                        {
                                            var TestedParam = 0;
                                            if (Condition.currentParam)
                                            {
                                                TestedParam = TargetCard.Life;
                                            }
                                            else
                                            {
                                                TestedParam = TargetCard.Life;
                                            }

                                            if (TestedParam == Condition.TestValue)
                                            {
                                                ConditionResult = true;
                                            }
                                            else
                                            {
                                                ConditionResult = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ConditionResult = false;
                                    }
                                    break;
                                case BasicParam.Armor:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentArmor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Armor;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentArmor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Armor;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        // ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Armor;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.Armor;
                                        }

                                        if (TestedParam == Condition.TestValue)
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
                                case BasicParam.Cost:
                                    if (TargetPermanentView != null)
                                    {
                                        // Ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.cost;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.cost;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.RefCardView.CurrentCost;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.cost;
                                        }

                                        if (TestedParam == Condition.TestValue)
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
                                case BasicParam.Power:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.currentPower;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.CardReferenceArchive.Power;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetEnemySlotView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetEnemySlotView.currentPower;
                                        }
                                        else
                                        {
                                            TestedParam = TargetEnemySlotView.PermanentData.Power;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        // ici non plus pas de current
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Power;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.Power;
                                        }

                                        if (TestedParam == Condition.TestValue)
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
                                case BasicParam.Durability:
                                    if (TargetPermanentView != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetPermanentView.Durability;
                                        }
                                        else
                                        {
                                            TestedParam = TargetPermanentView.MaxDurability;
                                        }

                                        if (TestedParam == Condition.TestValue)
                                        {
                                            ConditionResult = true;
                                        }
                                        else
                                        {
                                            ConditionResult = false;
                                        }
                                    }
                                    else if (TargetCard != null)
                                    {
                                        var TestedParam = 0;
                                        if (Condition.currentParam)
                                        {
                                            TestedParam = TargetCard.Durability;
                                        }
                                        else
                                        {
                                            TestedParam = TargetCard.MaxDurability;
                                        }

                                        if (TestedParam == Condition.TestValue)
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
                                default:
                                    ConditionResult = false;
                                    break;
                            }                        
                            break;

                        case DynamicCondition.ifTargetIsNotSelf:
                            if (TargetPermanentView != null)
                            {
                                PermanentView permanentView = Actionner.GetComponent<PermanentView>();
                                if (permanentView != null)
                                {
                                    if (TargetPermanentView == permanentView)
                                    {
                                        ConditionResult = false;
                                    }
                                    else
                                    {
                                        ConditionResult = true;
                                    }
                                }
                            }
                            else if (TargetEnemySlotView != null)
                            {
                                EnemySlotView enemySlotView = Actionner.GetComponent<EnemySlotView>();
                                if (enemySlotView != null)
                                {
                                    if (TargetEnemySlotView == enemySlotView)
                                    {
                                        ConditionResult = false;
                                    }
                                    else
                                    {
                                        ConditionResult = true;
                                    }
                                }                                
                            }
                            else if (TargetCard != null)
                            {
                                Card card = Actionner.GetComponent<Card>();
                                if (card != null)
                                {
                                    if (TargetCard == card)
                                    {
                                        ConditionResult = false;
                                    }
                                    else
                                    {
                                        ConditionResult = true;
                                    }
                                }                                  
                            }         
                            break;

                        default:
                            ConditionResult = true;
                            break;
                    }
                }
                else
                {
                    ConditionResult = true;
                }
            }

            if (ConditionResult == false)
            {
                return false;
            }
        }

        return true;
    }
}
