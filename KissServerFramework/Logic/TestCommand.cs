using CSharpLike;
using KissFramework;
using System;

namespace KissServerFramework
{
    public static class TestCommand
    {
        /// <summary>
        /// Process console command, that run in main thread.
        /// e.g. Input 'TestCommand aa "bb ""Cc" 1 1.5' in console, will output TestCommand:aa,bb "Cc,1,1.5
        /// </summary>
        /// <param name="args">The param of your command, case-sensitive</param>
        public static void OnCommand(string[] args)
        {
            string str = "TestCommand:";
            foreach (string arg in args)
                str += arg + ",";
            Logger.LogInfo(str);
        }
    }
}
