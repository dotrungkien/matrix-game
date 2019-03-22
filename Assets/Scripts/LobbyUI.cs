using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public Text userID;
    public Button createGameButton;

    void Start()
    {
        userID.text = Connection.GetInstance().userID;
        createGameButton.onClick.AddListener(Connection.GetInstance().CreateNewGame);
    }
}
