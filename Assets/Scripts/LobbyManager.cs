using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour, IListener
{
    void Start()
    {
        EventManager.GetInstance().AddListener(EVENT_TYPE.CREATE_GAME, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.CREATE_GAME:
                break;
            case EVENT_TYPE.JOIN_GAME:
                SceneManager.LoadScene("Game");
                break;
            default:
                break;
        }
    }
}
