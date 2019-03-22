using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Phoenix;

public class Connection : Singleton<Connection>, IListener
{
    [HideInInspector]
    public string userID;


    List<string> currentGames = new List<string>();
    private Socket socket = null;
    private Channel lobbyChannel;
    private Channel gameChannel;

    void Awake()
    {
        Debug.Log("this is awake");
        userID = PlayerPrefs.GetString("userID", "");
        if (userID == "")
        {
            Debug.Log("User ID not found. Create a new one.");
            userID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("userID", userID);
        }
        Debug.Log(string.Format("User ID: {0}", userID));
        SocketConnect();
        JoinLobby();
        // SceneManager.LoadScene("Game");
        // CreateNewGame(JoinGame);
        // JoinGame("parrot-wench-7508");
    }

    void SocketConnect()
    {
        socket = new Socket(new WebsocketSharpFactory());
        Socket.OnOpenDelegate onOpenCallback = () => Debug.Log("Socket on open.");
        Socket.OnClosedDelegate onCloseCallback = (code, message) => Debug.Log(string.Format("Socket on close. {0} {1}", code, message));
        // Socket.OnMessageDelegate onMessageCallback = (message) => Debug.Log(string.Format("Socket on message. {0}", message));
        Socket.OnErrorDelegate onErrorCallback = (message) => Debug.Log("Socket on error.");

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
        var param = new Dictionary<string, object> { };
        lobbyChannel.Join(param)
            .Receive(Reply.Status.Ok, reply => Debug.Log("Join lobbyChannel ok."))
            .Receive(Reply.Status.Error, reply => Debug.Log("Join lobbyChannel failed."))
            .Receive(Reply.Status.Timeout, reply => Debug.Log("Time out"));
    }

    public void CreateNewGame(Action<string, string> joinCallback = null)
    {
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
            string gameID = reply.response.GetValue("game_id").ToString();
            currentGames.Add(gameID);
            Debug.Log(string.Format("Create new game ok. {0}", gameID));
            SceneManager.LoadScene("Game");
            // if (joinCallback != null) joinCallback(gameID, "");
        })
        .Receive(Reply.Status.Error, reply => Debug.Log("Create new game failed."));
    }

    public void JoinGame(string gameID, string password = "")
    {
        gameChannel = socket.MakeChannel(string.Format("game:{0}", gameID));
        var param = new Dictionary<string, object>
        {
            {"nick", userID},
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
        StartListen();
        SceneManager.LoadScene("Game");

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
                Debug.Log(string.Format("Ready for game."));
            })
            .Receive(Reply.Status.Error, reply => Debug.Log("Not yet ready."));
    }

    void StartListen()
    {
        Debug.Log("Start Listen >>>>>>>>>>>>>>");
        gameChannel.On("game:new_piece", payload =>
        {
            Debug.Log("-----------------------new piece-----------------------");
            Debug.Log(payload);
        });
        gameChannel.On("game:player_joined", payload =>
        {
            Debug.Log("-----------------------player join-----------------------");
        });
        gameChannel.On("game:started", payload =>
        {
            Debug.Log("-----------------------Game started-----------------------");
            Debug.Log(payload);
        });
        gameChannel.On("game:stopped", payload =>
        {
            Debug.Log("-----------------------Game stopped-----------------------");
        });
        gameChannel.On("game:player_left", payload =>
        {
            Debug.Log("-----------------------player left-----------------------");

        });
        gameChannel.On("game:over", payload =>
        {
            Debug.Log("-----------------------game over-----------------------");

        });
        gameChannel.On("game:place_piece", payload =>
        {
            Debug.Log("-----------------------place piece-----------------------");
            Debug.Log(payload);
        });
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("userID");
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.CREATE_GAME:
                SceneManager.LoadScene("Game");
                break;
            case EVENT_TYPE.JOIN_GAME:
                SceneManager.LoadScene("Game");
                break;
            default:
                break;
        }
    }
}
