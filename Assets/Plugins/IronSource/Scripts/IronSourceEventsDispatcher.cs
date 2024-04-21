using System;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceEventsDispatcher : MonoBehaviour
{
    private static IronSourceEventsDispatcher instance;

    // Queue For Events
    private static readonly Queue<Action> ironSourceExecuteOnMainThreadQueue = new();

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // dispatch events on the main thread when the queue is bigger than 0
        while (ironSourceExecuteOnMainThreadQueue.Count > 0)
        {
            Action IronSourceDequeuedAction = null;
            lock (ironSourceExecuteOnMainThreadQueue)
            {
                try
                {
                    IronSourceDequeuedAction = ironSourceExecuteOnMainThreadQueue.Dequeue();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            if (IronSourceDequeuedAction != null) IronSourceDequeuedAction.Invoke();
        }
    }

    public void OnDisable()
    {
        instance = null;
    }

    public static void executeAction(Action action)
    {
        lock (ironSourceExecuteOnMainThreadQueue)
        {
            ironSourceExecuteOnMainThreadQueue.Enqueue(action);
        }
    }

    public void removeFromParent()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer &&
            Application.platform != RuntimePlatform.Android) Destroy(this);
    }

    public static void initialize()
    {
        if (isCreated()) return;

        // Add an invisible game object to the scene
        var obj = new GameObject("IronSourceEventsDispatcher");
        obj.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(obj);
        instance = obj.AddComponent<IronSourceEventsDispatcher>();
    }

    public static bool isCreated()
    {
        return instance != null;
    }
}