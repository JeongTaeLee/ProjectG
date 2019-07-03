﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine.Events;

public class ChattingChannel
{
    public const string All = "AL";
    public const string Fight = "FI";
    public const string Friend = "FR";

    public static string[] AllChannels
    {
        get => new string[] { All, Fight, Friend };
    }
}

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

public partial class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance = null;

    private ChatClient chatClient;

    public int historyMessageGetCount = 30;

    private string currentChannel = ChattingChannel.All;

    public StringEvent OnGetMessage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        chatClient.Service();
    }

    public void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }

    public void Initialize()
    {
        Application.runInBackground = true;

        currentChannel = ChattingChannel.All;

        chatClient = new ChatClient(this);

        chatClient.Connect(
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.GameVersion,
            new AuthenticationValues(PlayerManager.Instance.PlayerName));
    }

    public void ChangeChannel(string channelName)
    {
        currentChannel = channelName;
    }

    public void Send(string context)
    {
        if (chatClient.CanChat == false)
            return;

        if (string.IsNullOrWhiteSpace(context) == true)
            return;

        // 서버에 메시지 전송
        chatClient.PublishMessage(currentChannel, context);
    }

    public void SendWhisper(string userName, string context)
    {
        if (chatClient.CanChat == false)
            return;

        if (string.IsNullOrWhiteSpace(context) == true)
            return;

        chatClient.SendPrivateMessage(userName, context);
    }

    public void AddFriend(string userName)
    {
        chatClient.AddFriends(new string[] { userName });
    }

    public void RemoveFriend(string userName)
    {
        chatClient.RemoveFriends(new string[] { userName });
    }

    private void AddLine(string lineString)
    {
        OnGetMessage?.Invoke($"{lineString}\n");
    }

}
public partial class ChatManager : MonoBehaviour, IChatClientListener
{
    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        switch (level)
        {
            case DebugLevel.OFF:
                Debug.Log(message);
                break;
            case DebugLevel.ERROR:
                Debug.LogError(message);
                break;
            case DebugLevel.WARNING:
                Debug.LogWarning(message);
                break;
            case DebugLevel.INFO:
                Debug.Log(message);
                break;
            case DebugLevel.ALL:
                Debug.Log(message);
                break;
        }
    }

    void IChatClientListener.OnDisconnected()
    {
        AddLine("채팅 서버와의 연결이 끊어졌습니다.");
    }

    void IChatClientListener.OnConnected()
    {
        AddLine("채팅 서버에 연결되었습니다.");
        chatClient.Subscribe(ChattingChannel.AllChannels, historyMessageGetCount);
    }

    void IChatClientListener.OnChatStateChange(ChatState state)
    {
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            AddLine($"{senders[i]} : {messages[i].ToString()}");
        }
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        AddLine($"[{channelName}]{sender} >> {message}");
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
    }

    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
    }
}
