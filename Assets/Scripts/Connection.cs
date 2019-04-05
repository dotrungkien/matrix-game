using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phoenix;
using Newtonsoft.Json.Linq;

public class Connection : MonoBehaviour, IListener
{
    [HideInInspector]
    public string myID;

    public GameController gameController;
    public GameUI gameUI;
    public ListGames listGames;

    private Socket socket = null;
    private Channel lobbyChannel;
    private Channel gameChannel;
    private bool initGrids;

    void Start()
    {
        myID = PlayerPrefs.GetString("myID", "");
        if (myID == "")
        {
            myID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("myID", myID);
        }

        SocketConnect();
        initGrids = false;
        GameManager.GetInstance().isWatching = false;
        EventManager.GetInstance().AddListener(EVENT_TYPE.PLACE_PIECE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.JOIN_GAME, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.WATCH_GAME, this);
    }

    public void SocketDisconnect()
    {
        socket.Disconnect();
    }

    void SocketConnect()
    {
        socket = new Socket(new BestHTTPWebsocketFactory());
        Socket.OnOpenDelegate onOpenCallback = () =>
        {
            // Debug.Log("Socket on open.");
            JoinLobby();
        };
        Socket.OnClosedDelegate onCloseCallback = (code, message) => Debug.Log(string.Format("Socket on close. {0} {1}", code, message));
        // Socket.OnMessageDelegate onMessageCallback = (message) => Debug.Log(string.Format("Socket on message. {0}", message));
        Socket.OnErrorDelegate onErrorCallback = message =>
        {
            Debug.Log("Socket on error.");
        };

        socket.OnOpen += onOpenCallback;
        socket.OnError += onErrorCallback;
        socket.OnClose += onCloseCallback;
        // socket.OnMessage += onMessageCallback;

        Dictionary<string, string> socketArgument = new Dictionary<string, string>();
        socketArgument["id"] = myID;

        socket.Connect("wss://matrix.heasygame.com/socket", socketArgument);
    }

    void JoinLobby()
    {
        lobbyChannel = socket.MakeChannel("lobby");
        lobbyChannel.On("update_games", data =>
        {
            // Debug.Log(string.Format("------on update_games------ {0}", MessageSerialization.Serialize(data)));
            var games = data.payload["games"];
            if (listGames.gameObject.activeSelf) listGames.UpdateGames(games);
        });
        var param = new Dictionary<string, object> { };
        lobbyChannel.Join(param)
            .Receive(Reply.Status.Ok, reply =>
            {
                EventManager.GetInstance().PostNotification(EVENT_TYPE.SOCKET_READY);
                // Debug.Log("Join lobbyChannel ok.");
            })
            .Receive(Reply.Status.Error, reply => Debug.Log("Join lobbyChannel failed."))
            .Receive(Reply.Status.Timeout, reply => Debug.Log("Time out"));
    }

    public void CreateNewGame(string mode = "easy", string max_player = "2", string time_limit = "0", string password = "")
    {
        string gameID = "";
        var newGame = new Dictionary<string, object>
        {
            {"max_player", max_player},
            {"mode", mode},
            {"time_limit", time_limit},
            {"password", password}
        };

        lobbyChannel.Push("new_game", newGame)
        .Receive(Reply.Status.Ok, reply =>
        {
            gameID = reply.response.GetValue("game_id").ToString();
            JoinGame(gameID, password);
        })
        .Receive(Reply.Status.Error, reply => Debug.Log("Create new game failed."));
    }

    public void JoinGame(string gameID, string password = "")
    {
        GameChannelSetup(gameID);
        var username = PlayerPrefs.GetString("username", "noname");
        var param = new Dictionary<string, object>
        {
            {"nick", username},
            {"password", password}
        };
        gameChannel.Join(param)
            .Receive(Reply.Status.Ok, reply =>
            {
                EventManager.GetInstance().PostNotification(EVENT_TYPE.JOIN_GAME_SUCCESS, null, gameID);
                Debug.Log(string.Format("Join game {0} ok.", gameID));
            })
            .Receive(Reply.Status.Error, reply =>
            {
                EventManager.GetInstance().PostNotification(EVENT_TYPE.JOIN_GAME_FAILED);
                Debug.Log(string.Format("Join game {0} failed.", gameID));
            })
            .Receive(Reply.Status.Timeout, reply => Debug.Log("Time out"));
    }

    public void GameChannelSetup(string gameID)
    {
        gameChannel = socket.MakeChannel(string.Format("game:{0}", gameID));
        gameChannel.On("game:new_piece", data =>
        {
            // Debug.Log(string.Format("------on new_piece------ {0}", MessageSerialization.Serialize(data)));
            var piece = data.payload["piece"];
            int[] pieceVal = piece.ToObject<int[]>();
            SoundManager.GetInstance().MakeClickSound();
            EventManager.GetInstance().PostNotification(EVENT_TYPE.NEW_PIECE, this, pieceVal);
        });
        gameChannel.On("game:player_joined", data =>
        {
            // Debug.Log(string.Format("------on player_joined------ {0}", MessageSerialization.Serialize(data)));
            var game = data.payload["game"];
            gameUI.timeLimit = (int)game["time_limit"];
            DrawBoards(game);
        });
        gameChannel.On("game:stopped", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
            EventManager.GetInstance().PostNotification(EVENT_TYPE.GAMEOVER);
        });
        gameChannel.On("game:player_left", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
            EventManager.GetInstance().PostNotification(EVENT_TYPE.PLAYER_LEFT);

        });
        gameChannel.On("game:over", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
            EventManager.GetInstance().PostNotification(EVENT_TYPE.GAMEOVER);

        });
        gameChannel.On("game:place_piece", data =>
        {
            var game = data.payload["game"]; ;
            string[] players = game["players"].ToObject<string[]>();
            string player_nick;

            // Debug.Log(string.Format("------on place_piece----- {0}", MessageSerialization.Serialize(data)));
            if (!initGrids && GameManager.GetInstance().isWatching)
            {
                Debug.Log("Connection on watching");
                gameUI.timeLimit = (int)game["time_limit"];
                string[] player_nicks = game["player_nicks"].ToObject<string[]>();
                var playerScores = new Dictionary<int, KeyValuePair<string, string>>();
                for (int i = 0; i < players.Length; i++)
                {
                    string player_id = players[i];
                    player_nick = player_nicks[i];
                    int point = game["points"][player_id].ToObject<int>();
                    playerScores[i] = new KeyValuePair<string, string>(player_nick, (string)game["points"][player_id]);
                    string game_id = game["id"].ToObject<string>();
                    Dictionary<string, int> grid = game["player_boards"][player_id]["grid"].ToObject<Dictionary<string, int>>();
                    GridState state = new GridState(
                            player_id, player_nick, point, game_id, grid
                        );
                    gameController.UpdateGrid(player_id, state);
                }
                EventManager.GetInstance().PostNotification(EVENT_TYPE.SCORE_CHANGE, null, playerScores);
                initGrids = true;
                return;
            }

            string sender = (string)data.payload["player_id"];
            int index = Array.IndexOf(players, sender);
            player_nick = (string)game["player_boards"][sender]["nick"];
            Dictionary<string, int> gridData = game["player_boards"][sender]["grid"].ToObject<Dictionary<string, int>>();
            string pointStr = (string)game["points"][sender];
            var playerScore = new KeyValuePair<string, string>(player_nick, pointStr);
            var param = new Dictionary<int, KeyValuePair<string, string>> { { index, playerScore } };
            EventManager.GetInstance().PostNotification(EVENT_TYPE.SCORE_CHANGE, null, param);
            var piece = data.payload["piece"];
            gameController.UpdateGridData(sender, gridData, piece);
        });
    }

    public void DrawBoards(JToken game)
    {
        string[] players = game["players"].ToObject<string[]>();
        string[] player_nicks = game["player_nicks"].ToObject<string[]>();
        var playerScores = new Dictionary<int, KeyValuePair<string, string>>();
        for (int i = 0; i < players.Length; i++)
        {
            string player_id = players[i];
            string player_nick = player_nicks[i];
            int point = game["points"][player_id].ToObject<int>();
            playerScores[i] = new KeyValuePair<string, string>(player_nick, (string)game["points"][player_id]);
            string game_id = game["id"].ToObject<string>();
            Dictionary<string, int> grid = game["player_boards"][player_id]["grid"].ToObject<Dictionary<string, int>>();
            GridState state = new GridState(player_id, player_nick, point, game_id, grid);
            gameController.UpdateGrid(player_id, state);
        }
        EventManager.GetInstance().PostNotification(EVENT_TYPE.SCORE_CHANGE, null, playerScores);
    }

    public void SetReady()
    {
        var param = new Dictionary<string, object>
        {
            {"ready", true}
        };
        gameChannel.Push("game:set_ready", param)
        .Receive(Reply.Status.Ok, reply =>
            {
                Debug.Log("Ready for game.");
            })
            .Receive(Reply.Status.Error, reply => Debug.Log("Not yet ready."));
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("myID");
    }


    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.PLACE_PIECE:
                int[] coord = (int[])param;
                var pieceCoord = new Dictionary<string, object>
                {
                    {"x", coord[0]},
                    {"y", coord[1] * 3},
                };

                gameChannel.Push("game:place_piece", pieceCoord);
                break;
            case EVENT_TYPE.JOIN_GAME:
                var gameInfo = (KeyValuePair<string, string>)param;
                string gameID = gameInfo.Key;
                string password = gameInfo.Value;
                JoinGame(gameID, password);
                break;
            case EVENT_TYPE.WATCH_GAME:
                GameManager.GetInstance().isWatching = true;
                JoinGame((string)param);
                break;
            default:
                break;
        }
    }
}
