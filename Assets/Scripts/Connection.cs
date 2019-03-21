using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phoenix;

public class Connection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var socketFactory = new WebsocketSharpFactory();
        var socket = new Socket(socketFactory);
        socket.OnOpen += () => Debug.Log("Connect successful");
        socket.OnMessage += (m) => Debug.Log(string.Format("On Message {0}", m));

        Dictionary<string, string> socketArgument = new Dictionary<string, string>();
        socketArgument["id"] = "HU6OY-oh";
        socketArgument["vsn"] = "2.0.0";

        socket.Connect("wss://matrix.heasygame.com/socket/websocket", socketArgument);
        var roomChannel = socket.MakeChannel("lobby");
        roomChannel.On(Message.InBoundEvent.phx_close, m => Debug.Log(m));
        roomChannel.On("after_join", m => Debug.Log(m));

        roomChannel.Join(null)
        .Receive(Reply.Status.Ok, r => Debug.Log(r))
        .Receive(Reply.Status.Error, r => Debug.Log(r));
    }
}
