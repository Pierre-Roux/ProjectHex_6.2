using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveHolder : Singleton<SaveHolder>
{
    public SaveFile saveFile;
    public int SaveProfile;

    public new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
