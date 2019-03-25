using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phoenix;
using Newtonsoft.Json.Linq;

public class Connection : MonoBehaviour
{
    [HideInInspector]
    public string userID;
    public GridManager[] grids;

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

    public void CreateNewGame(string max_player = "2", string mode = "easy", string time_limit = "0", string password = "")
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
            currentGames.Add(gameID);
            EventManager.GetInstance().PostNotification(EVENT_TYPE.CREATE_GAME);
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
            Debug.Log("On Player Joined");
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
            // {"topic":"game:shipshape-foul-545","event":"game:place_piece","ref":null,"payload":{"player_id":"HU6OY-oh","piece":{"y":0,"x":8,"values":[7,9,7]},"game":{"turn":1,"time_limit":0,"spectators":[],"spectator_nicks":{},"points":{"HU6OY-oh":0,"2f250712-f039-4665-aac2-7149738da179":0},"players":["2f250712-f039-4665-aac2-7149738da179","HU6OY-oh"],"player_states":{"HU6OY-oh":"ready","2f250712-f039-4665-aac2-7149738da179":"moving"},"player_nicks":["Kien","sâfsafs"],"player_boards":{"HU6OY-oh":{"points":0,"player_id":"HU6OY-oh","nick":"sâfsafs","grid":{"86":-1,"70":-1,"53":-1,"33":-1,"60":-1,"47":-1,"08":7,"05":-1,"51":-1,"01":-1,"38":-1,"48":-1,"56":-1,"62":-1,"21":-1,"03":-1,"22":-1,"20":-1,"66":-1,"30":-1,"02":-1,"76":-1,"41":-1,"43":-1,"31":-1,"72":-1,"84":-1,"88":-1,"23":-1,"00":-1,"83":-1,"35":-1,"27":-1,"18":9,"80":-1,"67":-1,"54":-1,"28":7,"65":-1,"25":-1,"63":-1,"85":-1,"17":-1,"15":-1,"40":-1,"11":-1,"45":-1,"57":-1,"68":-1,"74":-1,"52":-1,"73":-1,"44":-1,"04":-1,"13":-1,"55":-1,"50":-1,"06":-1,"46":-1,"82":-1,"32":-1,"77":-1,"34":-1,"07":-1,"64":-1,"26":-1,"81":-1,"16":-1,"36":-1,"42":-1,"12":-1,"71":-1,"75":-1,"14":-1,"24":-1,"87":-1,"78":-1,"37":-1,"10":-1,"58":-1,"61":-1},"game_id":"shipshape-foul-545"},"2f250712-f039-4665-aac2-7149738da179":{"points":0,"player_id":"2f250712-f039-4665-aac2-7149738da179","nick":"Kien","grid":{"86":-1,"70":-1,"53":-1,"33":-1,"60":-1,"47":-1,"08":-1,"05":-1,"51":-1,"01":-1,"38":-1,"48":-1,"56":-1,"62":-1,"21":-1,"03":-1,"22":-1,"20":-1,"66":-1,"30":-1,"02":-1,"76":-1,"41":-1,"43":-1,"31":-1,"72":-1,"84":-1,"88":-1,"23":-1,"00":-1,"83":-1,"35":-1,"27":-1,"18":-1,"80":-1,"67":-1,"54":-1,"28":-1,"65":-1,"25":-1,"63":-1,"85":-1,"17":-1,"15":-1,"40":-1,"11":-1,"45":-1,"57":-1,"68":-1,"74":-1,"52":-1,"73":-1,"44":-1,"04":-1,"13":-1,"55":-1,"50":-1,"06":-1,"46":-1,"82":-1,"32":-1,"77":-1,"34":-1,"07":-1,"64":-1,"26":-1,"81":-1,"16":-1,"36":-1,"42":-1,"12":-1,"71":-1,"75":-1,"14":-1,"24":-1,"87":-1,"78":-1,"37":-1,"10":-1,"58":-1,"61":-1},"game_id":"shipshape-foul-545"}},"piece_values":[7,9,7],"phase":"running","mode":"easy","max_player":2,"locked":false,"id":"shipshape-foul-545"}}}
            Debug.Log(MessageSerialization.Serialize(data));
            string sender = data.payload["player_id"].ToObject<string>();
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
