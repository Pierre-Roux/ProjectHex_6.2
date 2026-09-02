using FMODUnity;
using UnityEngine;

public class EnemySlotViewCreator : Singleton<EnemySlotViewCreator>
{
    public EnemySlotView SlotPrefab;
    [HideInInspector] public EnemyZoneView WeaponZone;
    [HideInInspector] public EnemyZoneView ShieldZone;
    [HideInInspector] public EnemyZoneView SupportZone;
    public EnemySlotView CreateEnemySlotViewCreator(EnemyPermanentData data, PermanentArea type, bool setup = false)
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
                Debug.Log("No Type For Enemy " + data.name);
                break;
        }
        if (Parent == null) return null;

        int childCount = Parent.transform.childCount;
        if (childCount >= CombatSystem.Instance.MaxPermEnemy)
        {
            return null;
        }

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

        EnemySlotView enemySlotView = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, Parent.transform);
        enemySlotView.PermanentData = data;
        enemySlotView.setup();
        enemySlotView.gameObject.name = data.name + " " + CombatSystem.Instance.Enemy_Permanents.Count;

        CombatSystem.Instance.Enemy_Permanents.Add(enemySlotView);

        WeaponZone.RepositionChildrenEnemySlotView();
        ShieldZone.RepositionChildrenEnemySlotView();
        SupportZone.RepositionChildrenEnemySlotViewCenterOut();

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
