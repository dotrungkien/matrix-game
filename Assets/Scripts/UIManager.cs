using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Text player1Score;
    public Text player2Score;
    private int currentPlayer1Score;
    private int currentPlayer2Score;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.GetInstance();
        currentPlayer1Score = gameManager.player1Score;
        currentPlayer2Score = gameManager.player2Score;
        player1Score.text = "Player1: " + currentPlayer1Score;
        player2Score.text = "Player2: " + currentPlayer2Score;
    }
    void Update()
    {
        if (gameManager.player1Score != currentPlayer1Score)
        {
            currentPlayer1Score = gameManager.player1Score;
            player1Score.text = "Player1: " + currentPlayer1Score;
        }
        if (gameManager.player2Score != currentPlayer1Score)
        {
            currentPlayer1Score = gameManager.player2Score;
            player2Score.text = "Player1: " + currentPlayer1Score;
        }
    }
}
