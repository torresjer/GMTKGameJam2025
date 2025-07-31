using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance 
    {
        get
        {
           
           if (instance == null)
             SetupInstance();
           return instance;
        }
    }

    protected virtual void Awake()
    {
        RemoveDuplicates();
    }
    public static void SetupInstance() 
    {
        instance = (T)FindFirstObjectByType(typeof(T));
        if (instance != null)
        {
            DontDestroyOnLoad(instance.gameObject);
            return;
        }
        
        GameObject gameobj = new GameObject();
        gameobj.name = typeof(T).Name;
        instance = gameobj.AddComponent<T>();

        DontDestroyOnLoad(gameobj);
    }
    public void RemoveDuplicates() 
    {
        if (instance == null) 
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) 
        {
            Destroy(gameObject);
        }
    }
}
