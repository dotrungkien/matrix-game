using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public Color myColor;
    [HideInInspector]
    public bool isWatching = false;
    public Transform tileSpawnPos;
}
