using KissFramework;

namespace KissServerFramework
{
    /// <summary>
    /// The class for transfer JSON object between server and client, whatever the client using WebSocket or Socket.
    /// Received JSON object from client by 'void OnMessage(JSONData jsonData)'. 
    /// Send JSON object to client by 'void Send(JSONData jsonData)'.
    /// </summary>
    public sealed class Player : PlayerBase
    {
        /// <summary>
        /// Loaded account instance
        /// </summary>
        public Account account;
        /// <summary>
        /// Received JSON object from client, that run in main thread.
        /// </summary>
        /// <param name="jsonData">The JSON object from client</param>
        public override void OnMessage(JSONData jsonData)
        {
            Logger.LogInfo("Player:OnMessage:" + jsonData.ToString());
            switch (jsonData.GetPacketType<PacketType>())
            {
                //Chat room system
                case PacketType.ChatRoomSend:
                    ChatRoomManager.Instance.SendMessage(this, jsonData["Msg"]);
                    break;
                case PacketType.ChatRoomEnter:
                    nickname = jsonData["Nickname"];
                    ChatRoomManager.Instance.PlayerEnter(this);
                    break;
                case PacketType.ChatRoomExit:
                    ChatRoomManager.Instance.PlayerEnter(this);
                    break;

                //Account system
                case PacketType.AccountLogin:
                    AccountManager.Instance.Login(jsonData, this);
                    break;
                case PacketType.AccountChangeNameAndPassword:
                    AccountManager.Instance.ChangeNameAndPassword(jsonData, this);
                    break;

                //Aircraft battle system
                case PacketType.AircraftBattleSendScore:
                    break;
                case PacketType.CB_AircraftBattleGetRank:
                    break;

                default:
                    Logger.LogError("Player : OnMessage : Unknown packet type");
                    break;
            }
        }
        /// <summary>
        /// Simplify callback a tips string
        /// </summary>
        /// <param name="tips">The tips string send to client</param>
        /// <param name="bPrintLog">Whether print a log in server</param>
        public void CallbackTip(string tips, bool bPrintLog = true)
        {
            if (bPrintLog)
                Logger.LogInfo("CallbackTip : " + tips);
            JSONData jsonData = JSONData.NewPacket(PacketType.CB_Tips);
            jsonData["tips"] = tips;
            Send(jsonData);
        }
        /// <summary>
        /// Simplify callback a error string
        /// </summary>
        /// <param name="error">The error string send to client</param>
        /// <param name="bPrintLog">Whether print a log in server</param>
        public void CallbackError(string error, bool bPrintLog = true)
        {
            if (bPrintLog)
                Logger.LogError("CallbackError : " + error);
            JSONData jsonData = JSONData.NewPacket(PacketType.CB_Error);
            jsonData["error"] = error;
            Send(jsonData);
        }
        /// <summary>
        /// When the player disconnect, that run in main thread.
        /// The client if not complete the close action,
        /// the server will check whether the WebSocket is dead in every 30 seconds.
        /// </summary>
        public override void OnDisconnect()
        {
            Logger.LogInfo("Player:OnDisconnect");
            ChatRoomManager.Instance.PlayerExit(this);
            if (account != null)
                LogManager.Instance.LogAccount(account.uid, 1, IP);
        }
        public string nickname = "";
        /// <summary>
        /// This player guid, you should change to your unique ID.
        /// </summary>
        public string guid = System.Guid.NewGuid().ToString("N");
        /// <summary>
        /// When player connected, that run in main thread
        /// </summary>
        public override void OnConnect()
        {
            Logger.LogInfo("Player:OnOpen");
        }
        /// <summary>
        /// The WebSocket occur error, that run in main thread
        /// </summary>
        public override void OnError(string msg)
        {
            Logger.LogInfo("Player:OnError:"+ msg);
        }
    }
}
