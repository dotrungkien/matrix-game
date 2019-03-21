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
        Socket.OnOpenDelegate onOpenCallback = () => Debug.Log("Connect Succesful!");
        Socket.OnErrorDelegate onErrorCallback = (e) => Debug.Log("Connect failed");
        List<string> onMessageData = new List<string>();
        Socket.OnMessageDelegate onMessageCallback = m => onMessageData.Add(m);

        socket.OnOpen += onOpenCallback;
        socket.OnMessage += onMessageCallback;
        socket.OnError += onErrorCallback;

        Dictionary<string, string> socketArgument = new Dictionary<string, string>();
        socketArgument["id"] = "HU6OY-oh";
        socketArgument["vsn"] = "2.0.0";

        socket.Connect("wss://matrix.heasygame.com/socket", socketArgument);

        var roomChannel = socket.MakeChannel("lobby");
        roomChannel.On(Message.InBoundEvent.phx_error, mes =>
        {
            Debug.Log(string.Format("loi cmnr"));
        });
        roomChannel.Join();
    }
}
