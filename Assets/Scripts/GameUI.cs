using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour, IListener
{
    public GameManager gameManager;
    public Connection connection;

    // public set usetname panel
    public GameObject setUsernamePanel;
    public InputField usernameInput;
    public Button setUsername;

    // lobby panel
    public Text usernameText;
    public GameObject lobbyPanel;
    public Button createGameButton;

    //game panel
    public GameObject gamePanel;
    public Text myScore;
    public Text otherScore;
    public GameObject gameOverPanel;
    public Button readyButton;
    public Button restartButton;
    public Button quitButton;

    void Start()
    {
        string username = PlayerPrefs.GetString("username", "");
        if (username == "")
        {
            setUsernamePanel.SetActive(true);
            setUsername.onClick.AddListener(SetName);
        }
        else
        {
            setUsernamePanel.SetActive(false);
            usernameText.text = username;
        }
        lobbyPanel.SetActive(true);
        createGameButton.onClick.AddListener(CreateGame);
        createGameButton.gameObject.SetActive(false);

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SOCKET_READY, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        readyButton.onClick.AddListener(Ready);
        restartButton.onClick.AddListener(gameManager.Restart);
        quitButton.onClick.AddListener(gameManager.Restart);
    }

    public string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
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
            case EVENT_TYPE.JOIN_GAME:
                lobbyPanel.SetActive(false);
                gamePanel.SetActive(true);
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
