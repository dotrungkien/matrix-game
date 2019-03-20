using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameUI : MonoBehaviour
{
    public Text player1Score;
    public Text player2Score;
    public GameObject gameOverPanel;
    public Text player1Result;
    public Text player2Result;

    private int currentPlayer1Score;
    private int currentPlayer2Score;
    GameManager gameManager;

    void Start()
    {
        gameOverPanel.SetActive(false);
        gameManager = GameManager.GetInstance();
        currentPlayer1Score = gameManager.player1Score;
        currentPlayer2Score = gameManager.player2Score;
        player1Score.text = "PLAYER 1: " + currentPlayer1Score;
        player2Score.text = "PLAYER 2: " + currentPlayer2Score;
    }
    void Update()
    {
        if (gameManager.player1Score != currentPlayer1Score)
        {
            currentPlayer1Score = gameManager.player1Score;
            player1Score.text = "PLAYER 1: " + currentPlayer1Score;
        }
        if (gameManager.player2Score != currentPlayer2Score)
        {
            currentPlayer2Score = gameManager.player2Score;
            player2Score.text = "PLAYER 2: " + currentPlayer2Score;
        }
    }

    public void OnGameOver()
    {
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
    }
}
