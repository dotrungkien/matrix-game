using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameInfo : MonoBehaviour
{
    public Button joinGameNormal;
    public Button joinGamePassword;
    public Button watchGameButton;
    public Text gameLabel;
    public Text spectatorsLabel;
    public Text timeLimitLabel;
    public Image lockedImage;
    public Image openImage;
    public Text playersLabel;
    public Text modeLabel;
    public Text turnLabel;


    private string gameID;
    private bool isLocked;
    private string password;

    void Start()
    {

        joinGameNormal.onClick.AddListener(JoinGameNormal);
        joinGamePassword.onClick.AddListener(JoinGamePassword);
        watchGameButton.onClick.AddListener(WatchGame);
    }

    public void Setup(string _gameID, int spectators, int timeLimit, bool locked, bool isFull, string playerText, string mode, int turn)
    {
        gameID = _gameID;
        gameLabel.text = _gameID;
        spectatorsLabel.text = "" + spectators;
        timeLimitLabel.text = "" + timeLimit;
        isLocked = locked;
        lockedImage.gameObject.SetActive(locked);
        openImage.gameObject.SetActive(!locked);
        joinGameNormal.gameObject.SetActive(!isFull && !locked);
        joinGamePassword.gameObject.SetActive(!isFull && locked);
        watchGameButton.gameObject.SetActive(isFull);
        playersLabel.text = playerText;
        if (mode == "easy")
        {
            modeLabel.text = "7 → 10";
        }
        else if (mode == "medium")
        {
            modeLabel.text = "6 → 10";
        }
        else if (mode == "hard")
        {
            modeLabel.text = "5 → 10";
        }
        turnLabel.text = "" + turn;
    }


    public void JoinGameNormal()
    {
        // SoundManager.GetInstance().MakeClickSound();
        JoinGame("");
        joinGameNormal.onClick.RemoveListener(JoinGameNormal);
    }

    public void JoinGamePassword()
    {
        EventManager.GetInstance().PostNotification(EVENT_TYPE.JOIN_GAME_PASSWORD, null, gameID);
    }

    public void JoinGame(string password)
    {
        var param = new KeyValuePair<string, string>(gameID, password);
        EventManager.GetInstance().PostNotification(EVENT_TYPE.JOIN_GAME, this, param);
    }

    public void WatchGame()
    {
        // SoundManager.GetInstance().MakeClickSound();
        EventManager.GetInstance().PostNotification(EVENT_TYPE.WATCH_GAME, this, gameID);
        Debug.Log(string.Format("Watching game {0}", gameID));
    }
}
