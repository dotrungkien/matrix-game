using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phoenix;

public class Connection : Singleton<Connection>
{
    [HideInInspector]
    public string userID;


    List<string> currentGames = new List<string>();
    private Channel channel;

    void Awake()
    {
        userID = PlayerPrefs.GetString("userid", "");
        if (userID == "")
        {
            Debug.Log("User ID not found. Create a new one.");
            userID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("userID", userID);
        }
        Debug.Log(string.Format("User ID: {0}", userID));

        var socketFactory = new WebsocketSharpFactory();
        var socket = new Socket(socketFactory);
        Socket.OnOpenDelegate onOpenCallback = () => Debug.Log("Socket connect ok.");
        Socket.OnErrorDelegate onErrorCallback = (e) => Debug.Log("Socket connect failed.");
        List<string> onMessageData = new List<string>();
        Socket.OnMessageDelegate onMessageCallback = m =>
        {
            onMessageData.Add(m);
        };

        socket.OnOpen += onOpenCallback;
        socket.OnMessage += onMessageCallback;
        socket.OnError += onErrorCallback;

        Dictionary<string, string> socketArgument = new Dictionary<string, string>();
        socketArgument["id"] = userID;

        socket.Connect("wss://matrix.heasygame.com/socket", socketArgument);

        Message afterJoinMessage = null;
        Message closeMessage = null;
        Message errorMessage = null;
        Message replyMessage = null;

        channel = socket.MakeChannel("lobby");
        channel.On(Message.InBoundEvent.phx_error, m =>
        {
            // Debug.Log(string.Format("Error message {0}", m));
            errorMessage = m;
        });
        channel.On(Message.InBoundEvent.phx_reply, m =>
        {
            // Debug.Log(string.Format("Reply message {0}", m));
            replyMessage = m;
        });
        channel.On(Message.InBoundEvent.phx_close, m =>
        {
            // Debug.Log(string.Format("Close message {0}", m));
            closeMessage = m;
        });
        channel.On("after_join", m =>
        {
            // Debug.Log(string.Format("After join message {0}", m));
            afterJoinMessage = m;
        });
        var param = new Dictionary<string, object> { };
        channel.Join(param)
            .Receive(Reply.Status.Ok, r => Debug.Log("Join channel ok."))
            .Receive(Reply.Status.Error, r => Debug.Log("Join channel failed."))
            .Receive(Reply.Status.Timeout, r => Debug.Log("Time out"));
    }

    public void CreateNewGame()
    {
        var newGame = new Dictionary<string, object>
        {
            {"max_player", "2"},
            {"mode", "easy"},
            {"time_limit", "0"},
            {"password", ""}
        };

        channel.Push("new_game", newGame)
        .Receive(Reply.Status.Ok, r =>
        {
            string gameID = r.response.GetValue("game_id").ToString();
            currentGames.Add(gameID);
            Debug.Log(string.Format("Create new game ok. {0}", gameID));
        })
        .Receive(Reply.Status.Error, r => Debug.Log("Create new game failed."));
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("userid");
    }
}
