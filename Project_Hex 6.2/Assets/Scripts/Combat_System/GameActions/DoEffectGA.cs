public class DoEffectGA : GameAction
{
    public Effect Effect { get; set; }

    public DoEffectGA(Effect effect)
    {
        Effect = effect;
    }
}
