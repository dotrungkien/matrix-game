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

        socket.Connect("wss://matrix.heasygame.com/socket/websocket", socketArgument);
    }
}
