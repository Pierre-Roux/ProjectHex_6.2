public enum TargetLimitations
{
    NULL,

    OnlyOwnerType,
    ExceptOwnerType,

    Param_More_Than_Value,
    Param_Less_Than_Value,
    Param_Equal_Value,

    // General
    Only_Activated,
    Only_SelectablePermanent,
}

[System.Serializable]
public class TargetLimitationInfo
{
    public TargetLimitations targetLimitations;
    public Enemy_Player_ENUM Owner;
    public KeyWordType keyWordType;
    public BasicParam Param;
    public int ParamValue = -1;
    public bool MandatoryLimitation;
    

    public TargetLimitationInfo(){}

    public TargetLimitationInfo(int intValue, KeyWordType KeyWordType, Enemy_Player_ENUM owner, BasicParam param, TargetLimitations TargetLimitations, bool mandatoryLimitation = true)
    {
        ParamValue = intValue;
        keyWordType = KeyWordType;
        Owner = owner;
        Param = param;
        targetLimitations = TargetLimitations;
        MandatoryLimitation = mandatoryLimitation;
    }
}

