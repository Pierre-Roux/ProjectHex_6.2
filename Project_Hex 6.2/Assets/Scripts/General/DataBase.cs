using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class DataBase : Singleton<DataBase>
{
    public List<CardData> GlobalCardList;
    
    public int Money;
    public int MaxMana;
    

    public PlayerData StartingPlayer;
    public List<GameObject> EnemiesDataBase;

    [HideInInspector] public List<CardData> INITIALDeckList;
    [HideInInspector] public List<CardData> DeckList;
    [HideInInspector] public int CurrentStage;

    //AudioSource
    [HideInInspector] public StudioEventEmitter AudioSource;

    //Option
    [HideInInspector] public float MasterVolume;

    //For fight
    public bool IsElite;
    [HideInInspector] public int CoreLife;
    [HideInInspector] public int BaseCoreLife;

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
