using CSharpLike;
using KissFramework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace KissServerFramework
{
    public class GatewayServerInfo
    {
        public int ServerId;
        public string WebSocketURI;
        public string SocketHost;
        public int SocketPort;
        public string HttpURI;
        public override string ToString()
        {
            return KissJson.ToJson(this);
        }
    }
    public static class ReqGateway
    {
        static SortedDictionary<int, GatewayServerInfo> serverInfos = null;
        static GatewayServerInfo GetGatewayServerInfo(int serverId)
        {
            if (serverInfos == null)//Initalize if null
            {
                serverInfos = new SortedDictionary<int, GatewayServerInfo>();
                foreach (GatewayServerInfo one in Framework.config.serverInfos)
                    serverInfos[one.ServerId] = one;
            }
            if (serverInfos.Count == 0)//return null if not exist
                return null;
            if (serverInfos.TryGetValue(serverId, out GatewayServerInfo gatewayServerInfo))//return selected server infomation
                return gatewayServerInfo;
            return serverInfos.Values.Last();//return the last server infomation
        }
        /// <summary>
        /// Clear the dictionary of server information for force rebuild it.
        /// </summary>
        public static void Clear()
        {
            serverInfos = null;
        }
        /// <summary>
        /// The Client request server config, that run in main thread.
        /// </summary>
        [WebMethod(UriName = "ReqGateway")]
        public static string OnHttpReqGateway(string ip, bool isFreeVersion = false, int serverId = 0)
        {
            Logger.LogInfo($"ReqGateway : serverId = {serverId}, isFreeVersion = {isFreeVersion} from {ip}");

            //Build the server config by client params, that depend on your logic.
            JSONData cbJsonData = JSONData.NewDictionary();
            cbJsonData["serverInfo"] = KissJson.ToJSONData(GetGatewayServerInfo(serverId));
            cbJsonData["hotUpdateScriptFile"] = isFreeVersion ? Framework.config.hotUpdateScriptFileFree : Framework.config.hotUpdateScriptFile;
            return cbJsonData.ToJson();
        }
    }
}
