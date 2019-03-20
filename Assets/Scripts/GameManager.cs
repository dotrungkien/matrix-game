using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public GameObject tile1;
    public GameObject tile2;
    public Transform spawnPosition1;
    public Transform spawnPosition2;
    public int player1Score;
    public int player2Score;
    public int turn = 0;

    void Start()
    {
        Instantiate(tile1, spawnPosition1.position, Quaternion.identity, spawnPosition1);
        Instantiate(tile2, spawnPosition2.position, Quaternion.identity, spawnPosition2);
    }
    public int NextTurn()
    {
        turn = turn == 0 ? 1 : 0;
        return turn;
    }

    public void SpawnNewTile()
    {
        if (turn == 0)
        {
            Instantiate(tile1, spawnPosition1.position, Quaternion.identity, spawnPosition1);
        }
        if (turn == 1)
        {
            Instantiate(tile2, spawnPosition2.position, Quaternion.identity, spawnPosition2);
        }
    }
}
