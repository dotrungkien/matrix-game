using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phoenix;
using Newtonsoft.Json.Linq;

public class Connection : Singleton<Connection>
{
    [HideInInspector]
    public string userID;


    List<string> currentGames = new List<string>();
    private Socket socket = null;
    private Channel lobbyChannel;
    private Channel gameChannel;

    void Start()
    {
        userID = PlayerPrefs.GetString("userID", "");
        if (userID == "")
        {
            userID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("userID", userID);
        }
        SocketConnect();
    }

    void SocketConnect()
    {
        socket = new Socket(new BestHTTPWebsocketFactory());
        Socket.OnOpenDelegate onOpenCallback = () =>
        {
            Debug.Log("Socket on open.");
            JoinLobby();
        };
        Socket.OnClosedDelegate onCloseCallback = (code, message) => Debug.Log(string.Format("Socket on close. {0} {1}", code, message));
        // Socket.OnMessageDelegate onMessageCallback = (message) => Debug.Log(string.Format("Socket on message. {0}", message));
        Socket.OnErrorDelegate onErrorCallback = message => Debug.Log("Socket on error.");

        socket.OnOpen += onOpenCallback;
        socket.OnError += onErrorCallback;
        socket.OnClose += onCloseCallback;
        // socket.OnMessage += onMessageCallback;

        Dictionary<string, string> socketArgument = new Dictionary<string, string>();
        socketArgument["id"] = userID;

        socket.Connect("wss://matrix.heasygame.com/socket", socketArgument);
    }

    void JoinLobby()
    {
        lobbyChannel = socket.MakeChannel("lobby");
        lobbyChannel.On("current_games", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
        });
        var param = new Dictionary<string, object> { };
        lobbyChannel.Join(param)
            .Receive(Reply.Status.Ok, reply => Debug.Log("Join lobbyChannel ok."))
            .Receive(Reply.Status.Error, reply => Debug.Log("Join lobbyChannel failed."))
            .Receive(Reply.Status.Timeout, reply => Debug.Log("Time out"));
    }

    public void CreateNewGame()
    {
        string gameID = "";
        var newGame = new Dictionary<string, object>
        {
            {"max_player", "2"},
            {"mode", "easy"},
            {"time_limit", "0"},
            {"password", ""}
        };

        lobbyChannel.Push("new_game", newGame)
        .Receive(Reply.Status.Ok, reply =>
        {
            gameID = reply.response.GetValue("game_id").ToString();
            currentGames.Add(gameID);
            JoinGame(gameID);
            EventManager.GetInstance().PostNotification(EVENT_TYPE.CREATE_GAME);
            JoinGame(gameID, "");
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
                Debug.Log(string.Format("Join game {0} ok.", gameID));
                EventManager.GetInstance().PostNotification(EVENT_TYPE.JOIN_GAME);
            })
            .Receive(Reply.Status.Error, reply => Debug.Log(string.Format("Join game {0} failed.", gameID)))
            .Receive(Reply.Status.Timeout, reply => Debug.Log("Time out"));
    }

    public void GameChannelSetup(string gameID)
    {
        gameChannel = socket.MakeChannel(string.Format("game:{0}", gameID));
        gameChannel.On("game:new_piece", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
            var piece = data.payload["piece"];
            int[] pieceVal = piece.ToObject<int[]>();
            EventManager.GetInstance().PostNotification(EVENT_TYPE.NEW_PIECE, this, pieceVal);
        });
        gameChannel.On("game:player_joined", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
        });
        gameChannel.On("game:started", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
        });
        gameChannel.On("game:stopped", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
        });
        gameChannel.On("game:player_left", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));

        });
        gameChannel.On("game:over", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));

        });
        gameChannel.On("game:place_piece", data =>
        {
            Debug.Log(MessageSerialization.Serialize(data));
        });
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
        PlayerPrefs.DeleteKey("userID");
    }
}
