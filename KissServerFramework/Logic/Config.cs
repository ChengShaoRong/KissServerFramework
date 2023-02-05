
using KissFramework;
using System.Collections.Generic;

namespace KissServerFramework
{
    /// <summary>
    /// The config load from JSON file(Environment.CurrentDirectory + "/" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".json").
    /// The field of ConfigBase is internal use by FrameworkBase.
    /// You can custom this class and modify the JSON file.
    /// Here are the sample read the 'welcomeMsg' of this class from JSON file:
    /// string msg = Framework.config.welcomeMsg;
    /// </summary>
    public class Config : ConfigBase
    {
        /// <summary>
        /// sample for config JSON : {"welcomeMsg":"Welcome [{0}] to chat room."}
        /// </summary>
        public string welcomeMsg = "Welcome [{0}] to chat room.";
        /// <summary>
        /// sample for config JSON : {"goodbyeMsg":"[{0}] leave the chat room."}
        /// </summary>
        public string goodbyeMsg = "[{0}] leave the chat room.";
        /// <summary>
        /// sample for config JSON : {"aircraftBattleRankCount":100}
        /// </summary>
        public int aircraftBattleRankCount = 100;
        /// <summary>
        /// sample for config JSON : {"sampleInt":10}
        /// </summary>
        public int sampleInt = 123;
        /// <summary>
        /// sample for config JSON : {"sampleStringList":["a","abc","cba"]}
        /// </summary>
        public List<string> sampleStringList = new List<string>();
        /// <summary>
        /// sample for config JSON : {"sampleIntList":[1,2,3]}
        /// </summary>
        public List<int> sampleIntList = new List<int>();
        /// <summary>
        /// sample for config JSON : {"sampleStringIntDictionary":{"aa":1.2,"bb":3.2}}
        /// </summary>
        public Dictionary<string, float> sampleStringIntDictionary = new Dictionary<string, float>();
        /// <summary>
        /// sample for config JSON : {"sampleStringStringDictionary":{"aa":"xyz","bb":"abc"}}
        /// </summary>
        public Dictionary<string, string> sampleStringStringDictionary = new Dictionary<string, string>();
        /// <summary>
        /// How many seconds the account data keep in cache after account offline.
        /// </summary>
        public int accountCacheTime = 60;
    }
}
