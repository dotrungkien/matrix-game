using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    [HideInInspector]
    public string pName;

    public Text playerName;
    public Text playerScore;

    public void UpdatePlayer(string name, string score)
    {
        pName = name;
        playerName.text = name;
        playerScore.text = score;
    }
}
