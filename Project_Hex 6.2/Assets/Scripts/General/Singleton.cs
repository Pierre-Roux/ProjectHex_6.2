using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    //public static T Instance { get; private set; }

    public static T Instance
    {
        get
        {
            //Debug.Log($"👁 First access to {typeof(T)}.Instance at {Time.time} from {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod()}");
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    Debug.LogError($"❌ Singleton of type {typeof(T)} not found in scene.");
                }
            }

            return _instance;
        }
    }
    public static T _instance;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this as T;
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
        Destroy(gameObject);
    }

    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }

}
