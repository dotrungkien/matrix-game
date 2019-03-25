using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListGames : MonoBehaviour
{
    public List<string> games = new List<string>();
    public SimpleObjectPool objectPool;
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
        for (int i = 0; i < games.Count; i++)
        {
            string gameID = games[i];
            GameObject newGameInfo = objectPool.GetObject();
            newGameInfo.transform.SetParent(contentPanel);

            GameInfo gameInfo = newGameInfo.GetComponent<GameInfo>();
            gameInfo.Setup(gameID);
        }
    }
}
