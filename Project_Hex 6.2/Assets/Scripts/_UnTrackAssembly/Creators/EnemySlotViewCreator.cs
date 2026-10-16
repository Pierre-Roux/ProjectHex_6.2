using FMODUnity;
using UnityEngine;

public class EnemySlotViewCreator : Singleton<EnemySlotViewCreator>
{
    public EnemySlotView SlotPrefab;

    public EnemySlotView CreateEnemySlotViewCreator(EnemyPermanentData data, bool setup = false, SlotView slotView = null)
    {
        GameObject Parent = null;

        if (slotView != null)
        {
            Parent = slotView.gameObject;
        }
        else
        {
            slotView = SlotSystem.Instance.GetFreeSlot(Enemy_Player_ENUM.Enemy);
            if (slotView != null)
                Parent = slotView.gameObject;
        }

        if (Parent == null) return null;

        if (!setup)
        {
            if (!AudioManager.Instance.IsValid(data.SummonEPermanentSound))
            {
                RuntimeManager.PlayOneShot(AudioManager.Instance.SummonEPermanentSound);
            }
            else
            {
                RuntimeManager.PlayOneShot(data.SummonEPermanentSound);
            }
        }

        EnemySlotView enemySlotView = Instantiate(SlotPrefab, Parent.transform.position, Quaternion.identity, Parent.transform);
        enemySlotView.PermanentData = data;
        enemySlotView.setup();
        enemySlotView.gameObject.name = data.name + " " + CombatSystem.Instance.Enemy_Permanents.Count;

        slotView.enemySlotView = enemySlotView;

        CombatSystem.Instance.Enemy_Permanents.Add(enemySlotView);

        if (setup == true)
        {
            GameEventSystem.Instance.ManageEffects(null, null, enemySlotView, true);
        }
        else
        {
            GameEventSystem.Instance.ManageEffects(null, null, enemySlotView);
        }

        EventInfo eventInfo = new EventInfo(Events.WhenPermaETB, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, enemySlotView);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        return enemySlotView;
    }
}
