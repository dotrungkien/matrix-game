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
        Socket.OnOpenDelegate onOpenCallback = () => Debug.Log("Socket connect succesfully!");
        Socket.OnErrorDelegate onErrorCallback = (e) => Debug.Log("Socket connect failed");
        List<string> onMessageData = new List<string>();
        Socket.OnMessageDelegate onMessageCallback = m =>
        {
            Debug.Log(m);
            onMessageData.Add(m);
        };

        socket.OnOpen += onOpenCallback;
        socket.OnMessage += onMessageCallback;
        socket.OnError += onErrorCallback;

        Dictionary<string, string> socketArgument = new Dictionary<string, string>();
        socketArgument["id"] = "ahihikienday";
        socketArgument["vsn"] = "2.0.0";

        socket.Connect("ws://matrix.heasygame.com/socket", socketArgument);

        Message afterJoinMessage = null;
        Message closeMessage = null;
        Message errorMessage = null;
        Message replyMessage = null;

        var roomChannel = socket.MakeChannel("lobby");
        roomChannel.On(Message.InBoundEvent.phx_error, m =>
        {
            Debug.Log(string.Format("Error message {0}", m));
            errorMessage = m;
        });
        roomChannel.On(Message.InBoundEvent.phx_reply, m =>
        {
            Debug.Log(string.Format("Reply message {0}", m));
            replyMessage = m;
        });
        roomChannel.On(Message.InBoundEvent.phx_close, m =>
        {
            Debug.Log(string.Format("Close message {0}", m));
            closeMessage = m;
        });
        roomChannel.On("after_join", m =>
        {
            Debug.Log(string.Format("After join message {0}", m));
            afterJoinMessage = m;
        });
        var param = new Dictionary<string, object> { };
        roomChannel.Join(param)
            .Receive(Reply.Status.Ok, r => Debug.Log(string.Format("Join Channel Succesfully")))
            .Receive(Reply.Status.Error, r => Debug.Log(string.Format("Join Channel Failed! {0}", r)))
            .Receive(Reply.Status.Timeout, r => Debug.Log("Time out"));
    }
}
