using KissFramework;

namespace KissServerFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            //Start main thread loop, normally you don't need to modify it
            new Framework().Run();
        }
        #region Check window exit
        static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    Framework.Running = false;
                    Logger.LogWarning("CTRL+C received!", false);
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    Framework.Running = false;
                    Logger.LogWarning("CTRL+BREAK received!", false);
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    Framework.Running = false;
                    Logger.LogWarning("Close event received!", false);
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    Framework.Running = false;
                    Logger.LogWarning("LogOff or shutdown event received!", false);
                    break;
                default:
                    return true;
            }
            System.Threading.Thread.Sleep(1000);
            return false;
        }
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        #endregion
    }
}
