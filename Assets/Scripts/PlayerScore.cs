using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    public Text playerName;
    public Text playerScore;

    public void UpdatePlayer(string name, string score)
    {
        playerName.text = name;
        playerScore.text = score;
    }
}
