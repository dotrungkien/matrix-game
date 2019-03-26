using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListGames : MonoBehaviour
{
    public List<string> games = new List<string>();
    public GameInfo gameInfoPrefab;
    public Transform contentPanel;

    void Start()
    {
        UpdateListGames();
    }

    public void UpdateGames(List<string> _games)
    {
        games = _games;
        UpdateListGames();
    }

    public void UpdateListGames()
    {
        foreach (Transform child in contentPanel.transform)
        {
            if (child != null) GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < games.Count; i++)
        {
            string gameID = games[i];
            GameInfo gameInfo = Instantiate(gameInfoPrefab, gameInfoPrefab.transform.position, Quaternion.identity, contentPanel);
            gameInfo.Setup(gameID);
        }
    }
}
