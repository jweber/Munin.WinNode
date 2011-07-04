using log4net;

namespace Munin.WinNode
{
    class Logging
    {
        static ILog _logInstance;

        public static ILog Logger
        {
            get
            {
                if (_logInstance == null)
                    _logInstance = LogManager.GetLogger(typeof(Logging));

                return _logInstance;
            }
        }
    }
}
