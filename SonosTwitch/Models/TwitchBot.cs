using System;
using System.Windows;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace SonosTwitch
{
    public class TwitchBot
    {
        public TwitchClient Client { get; }
	
        public TwitchBot(string login, string token)
        {
            try
            {
                ConnectionCredentials credentials =
                    new ConnectionCredentials(login, $"oauth:{token}");
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(10)
                };
                WebSocketClient customClient = new WebSocketClient(clientOptions);
                Client = new TwitchClient(customClient);
                //Here may write any channel
                Client.Initialize(credentials, login);
                Client.OnLog += Client_OnLog;
                Client.OnJoinedChannel += Client_OnJoinedChannel;
                Client.OnMessageReceived += Client_OnMessageReceived;
                Client.OnWhisperReceived += Client_OnWhisperReceived;
                //client.OnNewSubscriber += Client_OnNewSubscriber;
                Client.OnConnected += Client_OnConnected;
                
                Client.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
  
        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }
  
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }
  
        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            //Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            //client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        public delegate void CommandReceived(object sender, OnMessageReceivedArgs e);

        public event CommandReceived Notify;
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            try
            {
                if (e.ChatMessage.Message[0].ToString() == App.Setting.Prefix)
                {
                    string res =
                        App.Setting.DictionaryCommands[$"{e.ChatMessage.Message.Replace(App.Setting.Prefix, "")}"];
                    if (res != null)
                    {
                        if (Notify != null && (App.Setting.ReceiveEveryone || App.Setting.ReceiveFollower ||
                                               App.Setting.ReceiveSubscriber == e.ChatMessage.IsSubscriber))
                            Notify(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

        }
        
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Console.WriteLine(e.WhisperMessage.Message);
            if (e.WhisperMessage.Username == "trumeetration")
                Client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }
        
        /*private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }*/
    }
}