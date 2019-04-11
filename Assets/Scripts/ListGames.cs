using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ListGames : MonoBehaviour
{
    public JArray games;
    public GameInfo gameInfoPrefab;
    public Transform contentPanel;


    public void UpdateGames(JToken _games)
    {
        games = (JArray)_games;
        UpdateListGames();
    }

    public void UpdateListGames()
    {
        if (games == null) return;
        foreach (Transform child in contentPanel.transform)
        {
            if (child != null) GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < games.Count; i++)
        {
            var game = games[i];
            int turn = (int)game["turn"];
            string gameID = (string)game["id"];
            string[] spectators = game["spectators"].ToObject<string[]>();
            int timeLimit = (int)game["time_limit"];
            bool locked = (bool)game["locked"];
            string mode = (string)game["mode"];
            string[] players = game["players"].ToObject<string[]>();
            int maxPlayer = (int)game["max_player"];
            bool isFull = players.Length == maxPlayer;
            string playersText = string.Format("{0} / {1}", players.Length, maxPlayer);
            string[] player_nicks = game["player_nicks"].ToObject<string[]>();

            GameInfo gameInfo = Instantiate(gameInfoPrefab, gameInfoPrefab.transform.position, Quaternion.identity, contentPanel);
            gameInfo.Setup(gameID, spectators.Length, timeLimit, locked, isFull, playersText, mode, turn, player_nicks);
        }
    }
}
