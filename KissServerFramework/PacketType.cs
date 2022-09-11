
namespace KissServerFramework
{
    /// <summary>
    /// Packet type between client and server.
    /// The prefix without 'CB_' mean client send to sever,
    /// otherwise mean server send to client.
    /// </summary>
    public enum PacketType
    {
        //Common
        CB_Error,
        CB_Tips,

        //Account system
        AccountLogin = 100,
        CB_AccountLogin,
        AccountChangeNameAndPassword,
        CB_AccountChangeNameAndPassword,
        CB_KickAccount,

        //Chat room system
        ChatRoomEnter = 200,
        CB_ChatRoomEnter,
        ChatRoomExit,
        CB_ChatRoomExit,
        ChatRoomSend,
        CB_ChatRoomSend,
        CB_HistoryMsg,

        //Aircraft battle system
        AircraftBattleSendScore = 300,
        CB_AircraftBattleSendScore,
        AircraftBattleGetRank,
        CB_AircraftBattleGetRank,
    }
}
