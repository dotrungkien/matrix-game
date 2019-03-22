using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public string gameID;
    public Text userID;
    public Button createGameButton;
    public Button joinGameButton;

    void Start()
    {
        userID.text = Connection.GetInstance().userID;
        createGameButton.onClick.AddListener(() => Connection.GetInstance().CreateNewGame());
        joinGameButton.onClick.AddListener(() => Connection.GetInstance().JoinGame(gameID, ""));
    }
}
