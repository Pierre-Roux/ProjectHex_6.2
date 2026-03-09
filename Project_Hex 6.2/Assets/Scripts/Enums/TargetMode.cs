public enum TargetMode
{
    Manual,

    Self,
    Core,
    All,
    RDM,

    HighHP,
    LowHP,

    HighCost,
    LowCost,

    EffectParent_Targets,
    NULL,
}

[System.Serializable]
public class TargetModeInfo
{
    public TargetMode targetMode;
    public Enemy_Player_ENUM PlayerOrEnemy;
    public PermanentArea permanentArea;
    public KeyWordType keyWordType;

    public TargetModeInfo(){}

    public TargetModeInfo(KeyWordType KeyWordType, TargetMode TargetMode)
    {
        keyWordType = KeyWordType;
        targetMode = TargetMode;
    }
}