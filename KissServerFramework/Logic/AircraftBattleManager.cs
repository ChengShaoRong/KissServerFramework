using CSharpLike;
using KissFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KissServerFramework
{
    public class AircraftBattleManager : Singleton<AircraftBattleManager>
    {
        public class Ranking
        {
            public int uid;
            public string nickname;
            public int score;
            public DateTime scoreTime;
            [KissJsonDontSerialize]
            public string sortKey;

            public Ranking(Account account)
            {
                uid = account.uid;
                nickname = account.nickname;
                score = account.score;
                scoreTime = account.scoreTime;
                ResetSortKey();
                while (rankings.ContainsKey(sortKey))
                    ResetSortKey();
                rankings[sortKey] = this;
                rankingKeys[account.uid] = sortKey;
            }
            public void ResetSortKey()
            {
                sortKey = (int.MaxValue - score).ToString("D10")        //sort DESC
                    + scoreTime.Ticks.ToString("D19")                   //sort ASCE
                    + FrameworkBase.GetRand(99999).ToString("D5");
            }
        }
        static Ranking lastRanking = null;
        static SortedDictionary<string, Ranking> rankings = new SortedDictionary<string, Ranking>();
        static Dictionary<int, string> rankingKeys = new Dictionary<int, string>();
        public void OnChangeScore(Account account)
        {
            if (account.score > 0
                && (lastRanking == null || account.score >= lastRanking.score))
            {
                Ranking ranking;
                //remove it if was in ranking
                if (rankingKeys.ContainsKey(account.uid))
                {
                    string key = rankingKeys[account.uid];
                    ranking = rankings[key];
                    ranking.score = account.score;
                    ranking.scoreTime = account.scoreTime;
                    ranking.ResetSortKey();
                    rankings.Remove(key);

                    while (rankings.ContainsKey(ranking.sortKey))
                        ranking.ResetSortKey();
                    rankings[ranking.sortKey] = ranking;
                    rankingKeys[account.uid] = ranking.sortKey;
                }
                else
                {
                    new Ranking(account);
                }
                //remove last one if reach max count
                if (rankings.Count > Framework.config.aircraftBattleRankCount)
                    rankings.Remove(rankings.Keys.Last());
                //reset last one
                lastRanking = rankings.Values.Last();
            }
        }
        public void Initialize()
        {
            Account.Select($"SELECT * FROM `account` WHERE score > 0 ORDER BY score DESC,scoreTime LIMIT {Framework.config.aircraftBattleRankCount}",
                new List<MySqlParameter>(),
                (accounts, error) =>
                {
                    if (accounts.Count > 0)
                    {
                        for (int i = 0; i < accounts.Count; i++)
                        {
                            Account account = accounts[i];
                            AccountManager.Instance.GetAccount(ref account);
                            new Ranking(account);
                        }
                        lastRanking = rankings.Values.Last();
                    }
                });
        }
        public void GetRank(Player player)
        {
            JSONData jsonData = JSONData.NewPacket(PacketType.CB_AircraftBattleGetRank);
            JSONData ranks = JSONData.NewList();
            foreach (Ranking rank in rankings.Values)
                ranks.Add(KissJson.ToJSONData(rank));
            jsonData["ranks"] = ranks;

            player.Send(jsonData);
        }
    }
}
