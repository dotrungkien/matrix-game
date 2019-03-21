using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour, IListener
{
    public Text player1Score;
    public Text player2Score;
    public GameObject gameOverPanel;
    public Text player1Result;
    public Text player2Result;

    GameManager gameManager;

    void Start()
    {
        gameOverPanel.SetActive(false);
        gameManager = GameManager.GetInstance();
        player1Score.text = "PLAYER 1: " + gameManager.player1Score;
        player2Score.text = "PLAYER 2: " + gameManager.player2Score;
        EventManager.GetInstance().AddListener(EVENT_TYPE.PLAYER1_SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.PLAYER2_SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.PLAYER1_SCORE_CHANGE:
                player1Score.text = "PLAYER 1: " + gameManager.player1Score;
                break;
            case EVENT_TYPE.PLAYER2_SCORE_CHANGE:
                player2Score.text = "PLAYER 2: " + gameManager.player2Score;
                break;
            case EVENT_TYPE.GAMEOVER:
                if (gameManager.player1Score == gameManager.player2Score)
                {
                    player1Result.text = "DRAW";
                    player2Result.text = "DRAW";
                }
                else if (gameManager.player1Score > gameManager.player2Score)
                {
                    player1Result.text = "WIN";
                    player2Result.text = "LOSE";
                }
                else
                {
                    player1Result.text = "LOSE";
                    player2Result.text = "WIN";
                }

                gameOverPanel.SetActive(true);
                break;
            default:
                break;
        }
    }
}
