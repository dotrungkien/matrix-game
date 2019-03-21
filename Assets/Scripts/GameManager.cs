using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>, IListener
{
    public GameObject tile1;
    public GameObject tile2;
    public Transform spawnPosition1;
    public Transform spawnPosition2;
    public int player1Score;
    public int player2Score;

    public int turnCount = 0;
    public bool isGameOver = false;

    public int turn;
    void Start()
    {
        Instantiate(tile1, spawnPosition1.position, Quaternion.identity, spawnPosition1);
        Instantiate(tile2, spawnPosition2.position, Quaternion.identity, spawnPosition2);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
    }

    public int NextTurn()
    {
        turn = turn == 0 ? 1 : 0;
        return turn;
    }

    public void SpawnNewTile()
    {
        turnCount += 1;
        if (turn == 0)
        {
            Instantiate(tile1, spawnPosition1.position, Quaternion.identity, spawnPosition1);
        }
        if (turn == 1)
        {
            Instantiate(tile2, spawnPosition2.position, Quaternion.identity, spawnPosition2);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void Restart()
    {
        turn = 0;
        turnCount = 0;
        isGameOver = false;
        SceneManager.LoadScene("Game");
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.SCORE_CHANGE:
                if (sender.tag == Constants.GRID1_TAG) player1Score = (int)param;
                if (sender.tag == Constants.GRID2_TAG) player2Score = (int)param;
                break;
            case EVENT_TYPE.GAMEOVER:
                GameOver();
                break;
            default:
                break;
        }
    }
}
