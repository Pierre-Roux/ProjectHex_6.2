using System.Threading;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class PermanentViewCreator : Singleton<PermanentViewCreator>
{
    [SerializeField] private PermanentView PermanentViewPrefab;
    [SerializeField] public ZoneView WeaponZone;
    [SerializeField] public ZoneView ShieldZone;
    [SerializeField] public ZoneView SupportZone;

    public PermanentView CreatePermanentViewCreator(Card cardReference, PermanentArea type)
    {
        GameObject Parent = null;
        switch (type)
        {
            case PermanentArea.Weapon:
                Parent = WeaponZone.gameObject;
                break;
            case PermanentArea.Shield:
                Parent = ShieldZone.gameObject;
                break;
            case PermanentArea.Support:
                Parent = SupportZone.gameObject;
                break;
            default:
                Debug.Log("No Type For permanent " + cardReference.Title);
                break;
        }
        if (Parent == null) return null;

        int childCount = Parent.transform.childCount;
        if (childCount >= 9)
        {
            //Debug.Log($"[EnemySlotViewCreator] Cannot add {data.name} to {type} zone — already {childCount} slots (limit = 9)");
            return null;
        }
        
        if (!AudioManager.Instance.IsValid(cardReference.SummonPPermanentSound))
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.SummonPPermanentSound);
        }
        else
        {
            RuntimeManager.PlayOneShot(cardReference.SummonPPermanentSound);
        }

        PermanentView PermanentView = Instantiate(PermanentViewPrefab, Vector3.zero, Quaternion.identity, Parent.transform);
        PermanentView.transform.localScale = Vector3.zero;
        PermanentView.transform.DOScale(PermanentViewPrefab.transform.localScale, 0.15f);
        PermanentView.Setup(cardReference);
        PermanentView.gameObject.name = cardReference.Title + " " + CombatSystem.Instance.Player_Permanents.Count;

        CombatSystem.Instance.Player_Permanents.Add(PermanentView);

        WeaponZone.RepositionChildrenPermanentView();
        ShieldZone.RepositionChildrenPermanentView();
        SupportZone.RepositionChildrenPermanentViewCenterOut();

        foreach (var effect in cardReference.Effects)
        {
            int MultiHit = effect.MultiHit;
            if (MultiHit < 1) MultiHit = 1;
            for (int i = 0; i < MultiHit; i++)
            {
                // Vérifie Hollow
                bool canApply = (PermanentView.permaTypes.Contains(PermaTypes.Hollow) && effect.HollowEffect)
                            || (!PermanentView.permaTypes.Contains(PermaTypes.Hollow) && !effect.HollowEffect);
                if (!canApply) continue;

                // On démarre par l’effet cloné
                Effect clonedEffect = effect.Clone();

                while (clonedEffect != null)
                {
                    if (clonedEffect.Events == Events.Instant)
                    {
                        clonedEffect.Actionner = PermanentView.gameObject;
                        DoEffectGA performEffectGA = new(clonedEffect);
                        ActionSystem.Instance.AddReaction(performEffectGA);
                    }
                    else
                    {
                        if (clonedEffect.Events != Events.EnemyTurn &&
                            clonedEffect.Events != Events.Instant)
                        {
                            //Debug.Log("Register " + clonedEffect);
                            GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                        }
                    }

                    clonedEffect.Actionner = PermanentView.gameObject;

                    if (clonedEffect.Events != Events.OnActivate)
                    {
                        if (clonedEffect.LinkedEffect != null)
                        {
                            clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                        }
                        clonedEffect = clonedEffect.LinkedEffect;
                    }
                    else
                    {
                        clonedEffect = null;
                    }
                }
            }
        }

        TriggerEventGA triggerEventGA = new(Events.WhenPermaETB,null,PermanentView,null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        return PermanentView;
    }
}
