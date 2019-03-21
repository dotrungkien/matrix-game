using UnityEngine;
using System.Collections;


public enum EVENT_TYPE
{
    GAMEOVER,
    NEXT_PLAYER,
    PLAYER1_SCORE_CHANGE,
    PLAYER2_SCORE_CHANGE,
}

public interface IListener
{
    void OnEvent(EVENT_TYPE eventType, Component sender, object param = null);
}