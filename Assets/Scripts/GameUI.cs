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
    public GameObject readyPanel;
    public Button restartButton;
    public Button quitButton;
    public Button readyButton;

    void Start()
    {
        setUsername.onClick.AddListener(SetName);
        restartButton.onClick.AddListener(gameManager.Restart);
        quitButton.onClick.AddListener(gameManager.Restart);
        string username = PlayerPrefs.GetString("username", "");
        if (username == "")
        {
            setUsernamePanel.SetActive(true);
        }
        else
        {
            setUsernamePanel.SetActive(false);
        }

        usernameText.text = username;
        createGameButton.onClick.AddListener(CreateGame);
        readyButton.onClick.AddListener(Ready);
        createGameButton.gameObject.SetActive(false);

        lobbyPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SOCKET_READY, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
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
        readyPanel.SetActive(false);
        readyButton.gameObject.SetActive(false);
        connection.SetReady();
    }

    void SetName()
    {
        string username = usernameInput.text;
        if (username == "") return;
        SoundManager.GetInstance().MakeClickSound();
        PlayerPrefs.SetString("username", username);
        usernameText.text = username;
        setUsernamePanel.SetActive(false);
        lobbyPanel.SetActive(true);
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
