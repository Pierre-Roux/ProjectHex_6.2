using FMODUnity;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] public StudioEventEmitter MusicEmitter;

    [Header ("Music : ")]

    [SerializeField] public EventReference BackgroundMusic;
    [SerializeField] public EventReference VictoryMusic;
    [SerializeField] public EventReference DefeatMusic;

    [Header ("Son Généraux : ")]
    [SerializeField] public EventReference clickSound;
    [SerializeField] public EventReference PlayCardSound;
    [SerializeField] public EventReference CannotPlayCardSound;
    [SerializeField] public EventReference HoverCardSound;
    [SerializeField] public EventReference PlaySpellSound;
    [SerializeField] public EventReference SummonPPermanentSound;
    [SerializeField] public EventReference SummonEPermanentSound;
    [SerializeField] public EventReference CollateralSound;
    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference HollowDieSound;
    [SerializeField] public EventReference DiscardCardSound;
    [SerializeField] public EventReference DrawCardSound;
    [SerializeField] public EventReference TakeCardRewardSound;
    [SerializeField] public EventReference TakeMoneyRewardSound;
    [SerializeField] public EventReference BuyCardSound;


    [Header ("Son d'effet : ")]
    [SerializeField] public EventReference Effect_GainManaSound;
    [SerializeField] public EventReference Effect_DrawSound;
    [SerializeField] public EventReference Effect_DiscardSound;
    [SerializeField] public EventReference Effect_HealSound;
    [SerializeField] public EventReference Effect_ArmorSound;
    [SerializeField] public EventReference Effect_DealDamageSound;
    [SerializeField] public EventReference Effect_LifeLossSound;
    [SerializeField] public EventReference Effect_InvocSound;
    [SerializeField] public EventReference Effect_RefreshSound;
    [SerializeField] public EventReference Effect_SacSound;
    [SerializeField] public EventReference Effect_PredictSound;
    [SerializeField] public EventReference Effect_ShieldSound;
    [SerializeField] public EventReference Effect_UnShieldSound;
    [SerializeField] public EventReference Effect_AlterCostSound;
    [SerializeField] public EventReference Effect_AlterPowerSound;
    [SerializeField] public EventReference Effect_AlterDurabilitySound;
    [SerializeField] public EventReference Effect_AlterIntegritySound;
    [SerializeField] public EventReference Effect_AlterPowerGridSound;
    [SerializeField] public EventReference Effect_AddACopySound;
    [SerializeField] public EventReference Effect_DisableSound;
    [SerializeField] public EventReference Effect_EnableSound;

    void Start()
    {
        PlayMusic(BackgroundMusic);
    }

    public void PlayMusic(EventReference Music)
    {
        if (MusicEmitter == null)
        {
            MusicEmitter = gameObject.AddComponent<StudioEventEmitter>();
        }

        MusicEmitter.EventReference = Music;
        MusicEmitter.Play(); // Lance la musique (en boucle si ton event FMOD est en loop)
    }

    public void StopMusic()
    {
        if (MusicEmitter != null)
        {
            MusicEmitter.Stop();
        }
    }

    public void ChangeMusic(EventReference NewMusic)
    {
        StopMusic();
        PlayMusic(NewMusic);
    }

    public void PlayClickSound()
    {
        RuntimeManager.PlayOneShot(clickSound);
    }

    public void PlayMoneyRewardSound()
    {
        RuntimeManager.PlayOneShot(TakeMoneyRewardSound);
    }
    
    public bool IsValid(EventReference eventref)
    {
        return !eventref.IsNull && !string.IsNullOrEmpty(eventref.Path);
    }
}
