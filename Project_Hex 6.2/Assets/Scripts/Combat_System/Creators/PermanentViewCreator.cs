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

        GameEventSystem.Instance.ManageEffects(null, PermanentView, null);

        TriggerEventGA triggerEventGA = new(Events.WhenPermaETB,null,PermanentView,null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        return PermanentView;
    }
}
