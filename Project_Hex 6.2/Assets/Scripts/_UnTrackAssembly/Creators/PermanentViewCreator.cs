using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class PermanentViewCreator : Singleton<PermanentViewCreator>
{
    [SerializeField] private PermanentView PermanentViewPrefab;
    [SerializeField] public ZoneView WeaponZone;
    [SerializeField] public ZoneView ShieldZone;
    [SerializeField] public ZoneView SupportZone;

    public PermanentView CreatePermanentViewCreator(Card cardReference, PermanentArea type, bool setup = false)
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

        if (!setup)
        {
            if (!AudioManager.Instance.IsValid(cardReference.SummonPPermanentSound))
            {
                RuntimeManager.PlayOneShot(AudioManager.Instance.SummonPPermanentSound);
            }
            else
            {
                RuntimeManager.PlayOneShot(cardReference.SummonPPermanentSound);
            }
        }

        PermanentView PermanentView = Instantiate(PermanentViewPrefab, Vector3.zero, Quaternion.identity, Parent.transform);
        PermanentView.transform.localScale = Vector3.zero;
        PermanentView.transform.DOScale(PermanentViewPrefab.transform.localScale, 0.15f);
        PermanentView.gameObject.name = cardReference.Title + " " + CombatSystem.Instance.Player_Permanents.Count;
        PermanentView.Setup(cardReference);

        CombatSystem.Instance.Player_Permanents.Add(PermanentView);
        CombatSystem.Instance.CurrentPowerGrid += cardReference.GridCost;
        CombatSystem.Instance.UpdatePowerGridText();

        WeaponZone.RepositionChildrenPermanentView();
        ShieldZone.RepositionChildrenPermanentView();
        SupportZone.RepositionChildrenPermanentViewCenterOut();

        Debug.Log("je joue la carte creature -> " + PermanentView.CardReferenceArchive.Title);

        GameEventSystem.Instance.ManageEffects(null, PermanentView, null);

        EventInfo eventInfo = new EventInfo(Events.WhenPermaETB, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, PermanentView, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        return PermanentView;
    }
}
