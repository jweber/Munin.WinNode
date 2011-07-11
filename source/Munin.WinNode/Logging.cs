using log4net;

namespace Munin.WinNode
{
    public static class Logging
    {
        static ILog _logInstance;

        static Logging()
        {
            if (_logInstance == null)
                _logInstance = LogManager.GetLogger(typeof(Logging));
        }

        public static void Debug(string message, params object[] args)
        {
            _logInstance.DebugFormat(message, args);
        }

        public static void Info(string message, params object[] args)
        {
            _logInstance.InfoFormat(message, args);
        }

        public static void Warn(string message, params object[] args)
        {
            _logInstance.WarnFormat(message, args);
        }

        public static void Error(string message, params object[] args)
        {
            _logInstance.ErrorFormat(message, args);
        }

        public static void Fatal(string message, params object[] args)
        {
            _logInstance.FatalFormat(message, args);
        }
    }
}
