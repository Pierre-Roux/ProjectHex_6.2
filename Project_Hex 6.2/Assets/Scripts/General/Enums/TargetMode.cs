public enum TargetMode
{
    Manual,

    Self,
    Core,
    All,
    RDM,

    HighHP,
    LowHP,


    EffectParent_Targets,
    NULL,
}

[System.Serializable]
public class TargetModeInfo
{
    public TargetMode targetMode;
    public Enemy_Player_ENUM PlayerOrEnemy;
    public PermanentArea permanentArea;
    public PermaTypes PermaType;

    public TargetModeInfo(){}

    public TargetModeInfo(PermaTypes permaType, TargetMode TargetMode)
    {
        PermaType = permaType;
        targetMode = TargetMode;
    }
}