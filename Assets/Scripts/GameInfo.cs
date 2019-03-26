using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameInfo : MonoBehaviour
{
    public Button joinGameButton;
    public Text gameLabel;

    private string gameID;

    void Start()
    {
        joinGameButton.onClick.AddListener(JoinGame);
    }

    public void Setup(string _gameID)
    {
        gameID = _gameID;
        gameLabel.text = _gameID;
    }

    public void JoinGame()
    {
        Debug.Log("join game " + gameID);
        EventManager.GetInstance().PostNotification(EVENT_TYPE.JOIN_GAME, this, gameID);
    }
}
