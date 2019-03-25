﻿using UnityEngine;
using System.Collections;


public enum EVENT_TYPE
{
    SOCKET_READY,
    NEW_PIECE,
    PLACE_PIECE,
    JOIN_GAME,
    SCORE_CHANGE,
    PLAYER_LEFT,
    GAMEOVER,
    RESTART,
}

public interface IListener
{
    void OnEvent(EVENT_TYPE eventType, Component sender, object param = null);
}