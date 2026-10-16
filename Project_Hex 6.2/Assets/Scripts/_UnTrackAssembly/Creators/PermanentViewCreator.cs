using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class PermanentViewCreator : Singleton<PermanentViewCreator>
{
    [SerializeField] private PermanentView PermanentViewPrefab;

    public PermanentView CreatePermanentViewCreator(Card cardReference, bool setup = false, SlotView slotView = null, bool GhostMode = false)
    {
        GameObject Parent = null;

        if (slotView != null)
        {
            Parent = slotView.gameObject;
        }
        else
        {
            slotView = SlotSystem.Instance.GetFreeSlot(Enemy_Player_ENUM.Player);
            if (slotView != null)
                Parent = slotView.gameObject;
        }

        if (Parent == null) return null;

        int childCount = Parent.transform.childCount;
        if (childCount >= 9)
        {
            //Debug.Log($"[EnemySlotViewCreator] Cannot add {data.name} to {type} zone — already {childCount} slots (limit = 9)");
            return null;
        }

        if (!setup || !GhostMode)
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

        PermanentView PermanentView = Instantiate(PermanentViewPrefab, Parent.transform.position, Quaternion.identity, Parent.transform);
        PermanentView.transform.localScale = Vector3.zero;
        PermanentView.transform.DOScale(PermanentViewPrefab.transform.localScale, 0.15f);
        PermanentView.gameObject.name = cardReference.Title + " " + CombatSystem.Instance.Player_Permanents.Count + " " + Time.time.ToString();
        PermanentView.Setup(cardReference);

        if (!GhostMode)
        {
            slotView.permanentView = PermanentView;

            CombatSystem.Instance.Player_Permanents.Add(PermanentView);
            CombatSystem.Instance.CurrentPowerGrid += cardReference.GridCost;
            CombatSystem.Instance.UpdatePowerGridText();

            Debug.Log("je joue la carte creature -> " + PermanentView.CardReferenceArchive.Title);

            GameEventSystem.Instance.ManageEffects(null, PermanentView, null);

            EventInfo eventInfo = new EventInfo(Events.WhenPermaETB, Enemy_Player_ENUM.Player, KeyWordType.NULL);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, PermanentView, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            Debug.Log(" ------------------INVOC REMAP--------------------");
            EnemySystem.Instance.RemapTarget(true,false,PermanentView);
        }      
        
        return PermanentView;
    }
}
