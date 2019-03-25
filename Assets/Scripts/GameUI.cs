using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour, IListener
{
    public string gameID;
    public GameManager gameManager;
    public Connection connection;

    // lobby panel
    public GameObject lobbyPanel;
    public InputField usernameInput;
    public Button setUsername;
    public Button createGameButton;

    //game panel
    public GameObject gamePanel;
    public Text myScore;
    public Text otherScore;
    public GameObject gameOverPanel;
    public Button readyButton;
    public Button restartButton;

    void Start()
    {
        lobbyPanel.SetActive(true);
        createGameButton.onClick.AddListener(CreateGame);
        createGameButton.gameObject.SetActive(false);
        setUsername.onClick.AddListener(SetName);
        usernameInput.text = PlayerPrefs.GetString("username", "");

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SOCKET_READY, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        readyButton.onClick.AddListener(Ready);
        restartButton.onClick.AddListener(gameManager.Restart);
    }

    public void CreateGame()
    {
        SoundManager.GetInstance().MakeClickSound();
        connection.CreateNewGame();
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void Ready()
    {
        SoundManager.GetInstance().MakeClickSound();
        readyButton.gameObject.SetActive(false);
        connection.SetReady();
    }

    void SetName()
    {
        SoundManager.GetInstance().MakeClickSound();
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
                KeyValuePair<string, string> playerScore = (KeyValuePair<string, string>)param;
                if (playerScore.Key == PlayerPrefs.GetString("username", ""))
                {
                    myScore.text = string.Format("{0}: {1}", playerScore.Key, playerScore.Value);
                }
                else
                {
                    otherScore.text = string.Format("{0}: {1}", playerScore.Key, playerScore.Value);
                }

                break;
            case EVENT_TYPE.GAMEOVER:
                gameOverPanel.SetActive(true);
                break;
            default:
                break;
        }
    }
}
