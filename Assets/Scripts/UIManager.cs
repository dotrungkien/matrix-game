using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Text scoreText;
    private int currentScore;
    GridManager gridManager;

    void Start()
    {
        gridManager = GridManager.GetInstance();
        currentScore = gridManager.totalScore;
        scoreText.text = "Score: " + currentScore;
    }
    void Update()
    {
        if (gridManager.totalScore != currentScore)
        {
            currentScore = gridManager.totalScore;
            scoreText.text = "Score: " + currentScore;
        }

    }
}
