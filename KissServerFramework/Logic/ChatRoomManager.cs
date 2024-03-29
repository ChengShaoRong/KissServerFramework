﻿using CSharpLike;
using KissFramework;
using System;
using System.Collections.Generic;

namespace KissServerFramework
{
    public class ChatRoomManager : Singleton<ChatRoomManager>
    {
        /// <summary>
        /// Chat room update every 60 seconds.
        /// </summary>
        /// <param name="deltaTime">delta time since last update, in seconds</param>
        public void Update(float deltaTime)
        {
            Logger.LogInfo($"ChatRoomManager:players count={players.Count}, deltaTime={deltaTime:F2} seconds");
        }
        Dictionary<int, Player> players = new Dictionary<int, Player>();
        JSONData historyMsgs = JSONData.NewList();

        public void PlayerEnter(Player player)
        {
            if (player.account == null)
            {
                Logger.LogError("PlayerEnter:account null");
                return;
            }
            players[player.account.uid] = player;
            //get the lately the history messages
            if (historyMsgs.Count > 0)
            {
                JSONData historyMsg = JSONData.NewPacket(PacketType.CB_HistoryMsg);
                historyMsg["history"] = historyMsgs;
                player.Send(historyMsg);
            }

            //notify to all players
            JSONData enterMsg = JSONData.NewPacket(PacketType.CB_ChatRoomEnter);
            enterMsg["msg"] = string.Format(Framework.config.welcomeMsg, player.account.nickname);
            enterMsg["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            BroadcastToAllPlayer(enterMsg.ToJson());
        }
        public void PlayerExit(Player player)
        {
            if (player.account == null)
            {
                Logger.LogError("PlayerExit:account null");
                return;
            }
            if (players.ContainsKey(player.account.uid))
            {
                players.Remove(player.account.uid);
                JSONData exitMsg = JSONData.NewPacket(PacketType.CB_ChatRoomExit);
                exitMsg["msg"] = string.Format(Framework.config.goodbyeMsg, player.account.nickname);
                exitMsg["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                BroadcastToAllPlayer(exitMsg.ToJson());
            }
        }
        public void SendMessage(Player player, string msg)
        {
            if (player.account == null)
            {
                Logger.LogError("SendMessage:account null");
                return;
            }
            if (players.ContainsKey(player.account.uid))
            {
                JSONData sendMsg = JSONData.NewPacket(PacketType.CB_ChatRoomSend);
                sendMsg["nickname"] = player.account.nickname;
                sendMsg["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sendMsg["msg"] = msg;
                BroadcastToAllPlayer(sendMsg.ToJson());
                //Add to history
                historyMsgs.Add(sendMsg);
                //Just keep 20 messages;
                if (historyMsgs.Count > 20)
                    historyMsgs.RemoveAt(0);
            }
        }
        public void BroadcastToAllPlayer(string msg)
        {
            foreach (Player player in players.Values)
                player.Send(msg);
        }
    }
}
