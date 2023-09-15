using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>: MonoBehaviour where T : Singleton<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T) this;
        }
        else
        {
            enabled = false;
            gameObject.SetActive(false);
            Debug.LogError("Duplicate Singleton");
            Destroy(gameObject);
        }
    }
}
