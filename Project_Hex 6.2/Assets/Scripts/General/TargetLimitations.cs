public enum TargetLimitations
{
    NULL,

    Only_Player_Permanent,
    Only_Enemy_Permanent,
    Only_Type_Permanent,
    Only_ActivablePermanent,

    NO_Player_Core,
    NO_Enemy_Core,

    Permanent_HP,
    Permanent_HP_More_Than_Value,
    Permanent_HP_Less_Than_Value,

    Permanent_Endurance,
    Permanent_Endurance_More_Than_Value,
    Permanent_Endurance_Less_Than_Value,

    Card_Cost_Value,
    Card_Cost_More_Than_Value,
    Card_Cost_Less_Than_Value,

}

[System.Serializable]
public class TargetLimitationInfo
{
    public TargetLimitations targetLimitations;
    public PermaTypes PermaType;
    public int IntValue = -1;
    public bool MandatoryLimitation;

    public TargetLimitationInfo(){}

    public TargetLimitationInfo(int intValue, PermaTypes permaType, TargetLimitations TargetLimitations, bool mandatoryLimitation = true)
    {
        IntValue = intValue;
        PermaType = permaType;
        targetLimitations = TargetLimitations;
        MandatoryLimitation = mandatoryLimitation;
    }
}

