using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour, IListener
{
    public string gameID;
    public Connection connection;

    // lobby panel
    public GameObject lobbyPanel;
    public InputField usernameInput;
    public Button setUsername;
    public Button createGameButton;
    public Button joinGameButton;
    //game panel
    public GameObject gamePanel;
    public Text player1Score;
    public Text player2Score;
    public GameObject gameOverPanel;
    public Text player1Result;
    public Text player2Result;
    public Button readyButton;

    GameManager gameManager;

    void Start()
    {
        lobbyPanel.SetActive(true);
        createGameButton.onClick.AddListener(CreateGame);
        createGameButton.gameObject.SetActive(false);
        joinGameButton.onClick.AddListener(JoinGame);
        setUsername.onClick.AddListener(SetName);
        usernameInput.text = PlayerPrefs.GetString("username", "");

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameManager = GameManager.GetInstance();
        player1Score.text = "PLAYER 1: " + gameManager.player1Score;
        player2Score.text = "PLAYER 2: " + gameManager.player2Score;
        EventManager.GetInstance().AddListener(EVENT_TYPE.SOCKET_READY, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        readyButton.onClick.AddListener(Ready);
    }

    public void CreateGame()
    {
        connection.CreateNewGame();
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void JoinGame()
    {
        connection.JoinGame(gameID, "");
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void Ready()
    {
        readyButton.gameObject.SetActive(false);
        connection.SetReady();
    }

    void SetName()
    {
        PlayerPrefs.SetString("username", usernameInput.text);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.SOCKET_READY:
                createGameButton.gameObject.SetActive(true);
                break;
            case EVENT_TYPE.SCORE_CHANGE:
                if (sender.tag == Constants.GRID1_TAG) player1Score.text = string.Format("PLAYER 1: {0}", (int)param);
                if (sender.tag == Constants.GRID2_TAG) player2Score.text = string.Format("PLAYER 2: {0}", (int)param);
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
