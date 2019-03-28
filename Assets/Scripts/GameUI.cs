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
    public Button openCreateGame;
    public GameObject createGamePanel;
    public GameObject passwordPanel;
    public GameObject wrongPasswordAlert;
    public InputField passwordInput;
    public Button joinGame;

    //game panel
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject readyPanel;
    public Button restartButton;
    public Button quitButton;
    public Button readyButton;

    public PlayerScore[] players;

    private string currentGameID;

    void Start()
    {
        setUsername.onClick.AddListener(SetName);
        restartButton.onClick.AddListener(gameManager.Restart);
        quitButton.onClick.AddListener(() =>
        {
            connection.SocketDisconnect();
            gameManager.Restart();
        });
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
        openCreateGame.onClick.AddListener(OpenCreateGame);
        readyButton.onClick.AddListener(Ready);
        openCreateGame.gameObject.SetActive(false);

        readyPanel.SetActive(true);
        lobbyPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        createGamePanel.SetActive(false);
        passwordPanel.SetActive(false);
        wrongPasswordAlert.SetActive(false);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SOCKET_READY, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME_PASSWORD, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME_SUCCESS, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME_FAILED, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
    }

    public string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public void OpenCreateGame()
    {
        SoundManager.GetInstance().MakeClickSound();
        createGamePanel.SetActive(true);
    }

    public void JoinGamePassword(string gameID)
    {
        string password = passwordInput.text;
        connection.JoinGame(gameID, password);
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
        SoundManager.GetInstance().MakeClickSound();
        string username = usernameInput.text;
        if (username == "") return;
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
                openCreateGame.gameObject.SetActive(true);
                break;
            case EVENT_TYPE.JOIN_GAME_PASSWORD:
                string gameID = (string)param;
                joinGame.onClick.AddListener(() => JoinGamePassword(gameID));
                passwordPanel.SetActive(true);
                wrongPasswordAlert.SetActive(false);
                break;
            case EVENT_TYPE.JOIN_GAME_FAILED:
                wrongPasswordAlert.SetActive(true);
                break;
            case EVENT_TYPE.JOIN_GAME_SUCCESS:
                wrongPasswordAlert.SetActive(false);
                lobbyPanel.SetActive(false);
                gamePanel.SetActive(true);
                break;
            case EVENT_TYPE.SCORE_CHANGE:
                var playerScores = (Dictionary<int, KeyValuePair<string, string>>)param;
                foreach (KeyValuePair<int, KeyValuePair<string, string>> item in playerScores)
                {
                    int index = item.Key;
                    KeyValuePair<string, string> playerScore = item.Value;
                    if (!players[index].gameObject.activeSelf) players[index].gameObject.SetActive(true);
                    string player_nick = playerScore.Key;
                    players[index].UpdatePlayer(playerScore.Key, playerScore.Value);
                    if (!players[index].isListening)
                    {
                        players[index].GetComponent<Image>().color = gameManager.colors[index];
                        if (index != 0)
                        {
                            Button activeSelf = players[index].GetComponent<Button>();
                            activeSelf.onClick.AddListener(() =>
                            {
                                gameManager.ActiveGrid(player_nick);
                            });
                        }

                        players[index].isListening = true;
                    }
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
