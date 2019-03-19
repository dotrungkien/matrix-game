using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public GameObject tile;
    public Transform spawnPosition;

    public void SpawnNewTile()
    {
        Instantiate(tile, spawnPosition.position, Quaternion.identity, spawnPosition);
    }
}
