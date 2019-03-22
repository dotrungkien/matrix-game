using UnityEngine;
using System.Collections;


public enum EVENT_TYPE
{
    GAMEOVER,
    NEXT_PLAYER,
    NEW_PIECE,
    SCORE_CHANGE,
    CREATE_GAME,
    JOIN_GAME,
}

public interface IListener
{
    void OnEvent(EVENT_TYPE eventType, Component sender, object param = null);
}