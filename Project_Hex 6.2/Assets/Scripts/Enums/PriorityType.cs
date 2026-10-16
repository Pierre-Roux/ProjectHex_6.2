using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
public enum PriorityType
{
    NULL,

    KeyWord,

    Param_More_Than_Value,
    Param_Less_Than_Value,
    Param_Equal_Value,

    Highest_Param,
    Lowest_Param,

    Highest_rarity,
    Lowest_rarity,
}

[System.Serializable]
public class PriorityInfo
{
    public PriorityType priorityType;
    public Enemy_Player_ENUM Owner;
    public KeyWordType keyWordType;
    public BasicParam Param;
    public int ParamValue = -1;

    [field: SerializeReference,SR] public List<DynamicConditionInfo> TargetConditions = new List<DynamicConditionInfo>();

    public PriorityInfo(){}

    public PriorityInfo(int intValue, KeyWordType KeyWordType, Enemy_Player_ENUM owner, BasicParam param, PriorityType PriorityType)
    {
        ParamValue = intValue;
        keyWordType = KeyWordType;
        Owner = owner;
        Param = param;
        priorityType = PriorityType;
    }
}
