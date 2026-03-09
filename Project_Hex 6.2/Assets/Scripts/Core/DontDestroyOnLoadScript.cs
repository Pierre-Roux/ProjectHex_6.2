using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
