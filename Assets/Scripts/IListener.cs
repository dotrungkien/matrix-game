using UnityEngine;
using System.Collections;


public enum EVENT_TYPE
{
    SOCKET_READY,
    NEXT_PLAYER,
    NEW_PIECE,
    PLACE_PIECE,
    SCORE_CHANGE,
    CREATE_GAME,
    JOIN_GAME,
    GAMEOVER,
}

public interface IListener
{
    void OnEvent(EVENT_TYPE eventType, Component sender, object param = null);
}