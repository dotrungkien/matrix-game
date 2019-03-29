using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGame : MonoBehaviour
{
    public GameObject lobbyPanel;
    public GameObject gamePanel;
    public GameObject createGamePanel;
    public Connection connection;
    public Button[] modeButtons;
    public Button[] maxPlayerButtons;
    public Button[] timeLimitButtons;
    public Button randomPassword;
    public Button createGame;
    public Button close;
    public InputField passwordInput;

    private string mode = "easy";
    private string maxPlayers = "2";
    private string timeLimit = "0";

    private Color activeColor = new Color(1f, 0.5f, 0);
    private Color inactiveColor = Color.white;

    private void Start()
    {
        modeButtons[0].image.color = activeColor;
        maxPlayerButtons[0].image.color = activeColor;
        timeLimitButtons[0].image.color = activeColor;
        randomPassword.onClick.AddListener(RandomPassword);
        createGame.onClick.AddListener(CreateAndStart);
        close.onClick.AddListener(Close);
        for (int i = 0; i < 3; i++)
        {
            int j = i;
            modeButtons[i].onClick.AddListener(() => SelectMode(j));
            maxPlayerButtons[i].onClick.AddListener(() => SelectMaxPlayer(j));
            timeLimitButtons[i].onClick.AddListener(() => SelectTimeLimit(j));
        }
    }

    public void Close()
    {
        createGamePanel.SetActive(false);
    }
    public void CreateAndStart()
    {
        int limit;
        Int32.TryParse(timeLimit, out limit);
        connection.CreateNewGame(mode, maxPlayers, timeLimit, passwordInput.text);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void RandomPassword()
    {
        string pass = RandomString(6);
        passwordInput.text = pass;
    }

    public string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public void SelectMode(int idx)
    {

        if (idx == 0)
        {
            mode = "easy";
            modeButtons[0].image.color = activeColor;
            modeButtons[1].image.color = inactiveColor;
            modeButtons[2].image.color = inactiveColor;
        }
        else if (idx == 1)
        {
            mode = "medium";
            modeButtons[0].image.color = inactiveColor;
            modeButtons[1].image.color = activeColor;
            modeButtons[2].image.color = inactiveColor;
        }
        else if (idx == 2)
        {
            mode = "hard";
            modeButtons[0].image.color = inactiveColor;
            modeButtons[1].image.color = inactiveColor;
            modeButtons[2].image.color = activeColor;
        }
    }

    public void SelectMaxPlayer(int idx)
    {
        if (idx == 0)
        {
            maxPlayers = "2";
            maxPlayerButtons[0].image.color = activeColor;
            maxPlayerButtons[1].image.color = inactiveColor;
            maxPlayerButtons[2].image.color = inactiveColor;
        }
        else if (idx == 1)
        {
            maxPlayers = "3";
            maxPlayerButtons[0].image.color = inactiveColor;
            maxPlayerButtons[1].image.color = activeColor;
            maxPlayerButtons[2].image.color = inactiveColor;
        }
        else if (idx == 2)
        {
            maxPlayers = "4";
            maxPlayerButtons[0].image.color = inactiveColor;
            maxPlayerButtons[1].image.color = inactiveColor;
            maxPlayerButtons[2].image.color = activeColor;
        }
    }

    public void SelectTimeLimit(int idx)
    {
        if (idx == 0)
        {
            timeLimit = "0";
            timeLimitButtons[0].image.color = activeColor;
            timeLimitButtons[1].image.color = inactiveColor;
            timeLimitButtons[2].image.color = inactiveColor;
        }
        else if (idx == 1)
        {
            timeLimit = "15";
            timeLimitButtons[0].image.color = inactiveColor;
            timeLimitButtons[1].image.color = activeColor;
            timeLimitButtons[2].image.color = inactiveColor;
        }
        else if (idx == 2)
        {
            timeLimit = "30";
            timeLimitButtons[0].image.color = inactiveColor;
            timeLimitButtons[1].image.color = inactiveColor;
            timeLimitButtons[2].image.color = activeColor;
        }
    }
}

