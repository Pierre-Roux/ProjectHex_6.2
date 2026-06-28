using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class DataBase : Singleton<DataBase>
{
    public CardPool ColorLessCardPool;
    public CardPool ChoosedCardPool;
    public CardPool BossCardPool;
    public CardPool StarterPool;

    public List<Stage> Stages;

    public int Money;
    public int MaxMana;
    public int MaxHandCount;
    public int MaxPowerGrid;
    public int NBCardDrawAtStartTurn;
    public int NavOptionNumber;

    public int Common_Weigh;
    public int Uncommon_Weigh;
    public int Rare_Weigh;

    [HideInInspector] public PlayerData CurrentPlayer;
    [HideInInspector] public GameObject SelectedEnemy;

    [HideInInspector] public List<CardData> INITIALDeckList;
    [HideInInspector] public List<NavCardData> INITIALNavDeckList;
    [HideInInspector] public List<CardData> DeckList;
    [HideInInspector] public List<NavCardData> NavDeckList;

    [HideInInspector] public Stage CurrentStage;

    //AudioSource
    [HideInInspector] public StudioEventEmitter AudioSource;

    //Option
    [HideInInspector] public float MasterVolume;

    //For fight
    [HideInInspector] public int CoreLife;
    [HideInInspector] public int BaseCoreLife;
    [HideInInspector] public bool BossFight;

    public new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        UpdateOptions();
    }

    public void UpdateOptions()
    {
        if(PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
            Bus masterBus = RuntimeManager.GetBus("bus:/");
            masterBus.setVolume(MasterVolume);            
        }
    }
}
