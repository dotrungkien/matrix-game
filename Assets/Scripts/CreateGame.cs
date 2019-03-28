using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGame : MonoBehaviour
{
    public Button[] modeButtons;
    public Button[] maxPlayerButtons;
    public Button[] timeLimitButtons;

    private string mode;
    private string maxPlayers;
    private string timeLimit;

    private Color activeColor = new Color(01f, 0.5f, 0);
    private Color inactiveColor = Color.white;

    private void Start()
    {
        modeButtons[0].image.color = activeColor;
        maxPlayerButtons[0].image.color = activeColor;
        timeLimitButtons[0].image.color = activeColor;
        for (int i = 0; i < modeButtons.Length; i++)
        {
            modeButtons[i].onClick.AddListener(() => SelectMode(i));
        }
        for (int i = 0; i < maxPlayerButtons.Length; i++)
        {
            maxPlayerButtons[i].onClick.AddListener(() => SelectMaxPlayer(i));
        }
        for (int i = 0; i < timeLimitButtons.Length; i++)
        {
            timeLimitButtons[i].onClick.AddListener(() => SelectTimeLimit(i));
        }
    }
    public void SelectMode(int idx)
    {

        if (idx == 0)
        {
            mode = "easy";
            maxPlayerButtons[0].image.color = activeColor;
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

