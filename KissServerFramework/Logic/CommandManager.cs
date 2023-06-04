using CSharpLike;
using KissFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace KissServerFramework
{
    /// <summary>
    /// We put console command here.
    /// You can put your command mehtod ANYWHERE as you like.
    /// Is very easy to add console command:
    /// 1: Define method as static.
    /// 2: Add [CommandMethod] into your method.
    /// 3: Custom command name like '[CommandMethod(Command = "CustomName")]', if you don't set it, it will using current method name as command, and is case-insensitive.
    /// 4: You can has 0 parameter or some parameters. Support param type 'byte/sbyte/short/ushort/int/uint/DateTime/string/float/double/bool'
    /// </summary>
    public class CommandManager
    {
        /// <summary>
        /// Input 'testcommand aa "bb ""Cc" 1 1.5' in console
        /// will call TestCommand method in main thread, and will pass parameters as:
        /// param1 = "a";
        /// param2 = "bb \"Cc";
        /// param3 = 1;
        /// param4 = 1.5f;
        /// </summary>
        /// <param name="param1">Command parameter 1</param>
        /// <param name="param2">Command parameter 2</param>
        /// <param name="param3">Command parameter 3, it must type of interger</param>
        /// <param name="param4">Command parameter 3, it must type of float</param>
        [CommandMethod]
        public static void TestComman(string param1, string param2, int param3, float param4)
        {
            Logger.LogInfo($"TestComman {param1} {param2} {param3} {param4}");
        }
        [CommandMethod]
        public static void ReloadCsv()
        {
            Framework.Instance.InitializeCSV();
        }
        [CommandMethod]
        public static void Quit()
        {
            Framework.Running = false;
        }
        [CommandMethod]
        public static void Exit()
        {
            Framework.Running = false;
        }
        [CommandMethod]
        public static void ReloadWWW()
        {
            Framework.Instance.ForceCheckCacheFile();
        }
        [CommandMethod]
        public static void TestThread()
        {
            //We test read a file in thread, and then process the result.
            string strFile = "./KissFramework.dll";
            string result = "";
            new ThreadPoolEvent(() =>//This function run in thread. You can do some heavy work in thread here, such as IO operation.
            {
                Logger.LogInfo($"start read file {strFile}", false);//Print log in thread you must set the param 'useInMainThread' as false.
                byte[] buff = System.IO.File.ReadAllBytes(strFile);
                System.Threading.Thread.Sleep(1000);//We simulate that work take long time.
                result = $"{strFile} file length = {buff.Length}";
            },
            () =>//This function run in main thread. You can do some work after your work done.
                {
                Logger.LogInfo(result);
            });
        }
    }
}
