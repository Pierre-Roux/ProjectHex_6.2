using System.Collections.Generic;
using Codice.CM.Common;
using UnityEngine;

[System.Serializable]
public class PassiveVarGroup
{
    public GameObject owner;
    public int value;
    public BasicParam basicParam;
    public TargetModeInfo targetModeInfo;
    public List<DynamicConditionInfo> conditions;

    public PassiveVarGroup(GameObject Owner, int Value, BasicParam BasicParam, TargetModeInfo TargetModeInfo, List<DynamicConditionInfo> Conditions)
    {
        value = Value;
        basicParam = BasicParam;
        targetModeInfo = TargetModeInfo;
        conditions = Conditions;
        owner = Owner;
    }

}
