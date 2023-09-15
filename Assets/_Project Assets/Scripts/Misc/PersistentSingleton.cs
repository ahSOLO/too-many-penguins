using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSingleton<T>: MonoBehaviour where T : PersistentSingleton<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T) this;
            DontDestroyOnLoad(this);
        }
        else
        {
            enabled = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
