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
        // Instantiate(tile1, spawnPosition1.position, Quaternion.identity, spawnPosition1);
        // Instantiate(tile2, spawnPosition2.position, Quaternion.identity, spawnPosition2);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.NEW_PIECE, this);
    }

    public int NextTurn()
    {
        turn = turn == 0 ? 1 : 0;
        return turn;
    }

    public void SpawnNewTile(int[] piece)
    {
        turnCount += 1;
        GameObject tileObj;
        if (turn == 0)
        {
            tileObj = Instantiate(tile1, spawnPosition1.position, Quaternion.identity, spawnPosition1);

        }
        else
        {
            tileObj = Instantiate(tile2, spawnPosition2.position, Quaternion.identity, spawnPosition2);
        }
        Tile tile = tileObj.GetComponent<Tile>();
        tile.SetVal(piece);
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
            case EVENT_TYPE.NEW_PIECE:
                int[] pieceVal = (int[])param;
                Debug.Log(string.Format("New Piece {0} {1} {2}", pieceVal[0], pieceVal[1], pieceVal[2]));
                SpawnNewTile(pieceVal);
                break;
            default:
                break;
        }
    }
}
