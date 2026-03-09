using System.Collections.Generic;

public class ExecEnemyTurnActionOnceGA : GameAction
{
    public List<Effect> EffectsToExec { get; set; }

    public ExecEnemyTurnActionOnceGA(List<Effect> effectsToExec)
    {
        EffectsToExec = effectsToExec;
    } 
}
