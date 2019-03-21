using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class EventManager : Singleton<EventManager>
{
    private Dictionary<EVENT_TYPE, List<IListener>> listeners = new Dictionary<EVENT_TYPE, List<IListener>>();

    public void AddListener(EVENT_TYPE eventType, IListener Listener)
    {
        List<IListener> ListenList = null;

        if (listeners.TryGetValue(eventType, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }

        ListenList = new List<IListener>();
        ListenList.Add(Listener);
        listeners.Add(eventType, ListenList);
    }

    public void PostNotification(EVENT_TYPE eventType, Component sender = null, object param = null)
    {
        List<IListener> ListenList = null;

        if (!listeners.TryGetValue(eventType, out ListenList))
            return;

        for (int i = 0; i < ListenList.Count; i++)
        {
            if (!ListenList[i].Equals(null)) //If object is not null, then send message via interfaces
                ListenList[i].OnEvent(eventType, sender, param);
        }
    }
    public void RemoveEvent(EVENT_TYPE eventType)
    {
        listeners.Remove(eventType);
    }

    public void RemoveRedundancies()
    {
        Dictionary<EVENT_TYPE, List<IListener>> TmpListeners = new Dictionary<EVENT_TYPE, List<IListener>>();

        foreach (KeyValuePair<EVENT_TYPE, List<IListener>> Item in listeners)
        {
            for (int i = Item.Value.Count - 1; i >= 0; i--)
            {
                if (Item.Value[i].Equals(null))
                    Item.Value.RemoveAt(i);
            }

            if (Item.Value.Count > 0)
                TmpListeners.Add(Item.Key, Item.Value);
        }

        listeners = TmpListeners;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RemoveRedundancies();
    }
}