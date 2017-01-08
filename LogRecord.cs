using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace LDAP.ActiveDirectory
{
    /// <summary>
    /// Logging or disalbed
    /// </summary>
    public enum DB_MODE : int
    {
        DISABLE = 0,
        ENABLE = 1,
    }

    
    /// <summary>
    /// logging record
    /// </summary>
    public class LogRecord
    {
        private static DB_MODE DEBUG_MODE = DB_MODE.ENABLE;
        private LogRecord()
        {
        }

        public static DB_MODE Mode
        {
            set { DEBUG_MODE = value; }
            get { return DEBUG_MODE; }
        }
        private static string sLogFile = string.Format(@".\adlog{0}.log", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));

        public static void WriteLog(string sMsg)
        {
            if (DEBUG_MODE == DB_MODE.ENABLE)
            {
                // Write log msg
                try
                {
                    using (StreamWriter sr = new StreamWriter(sLogFile, true))
                    {
                        sr.WriteLine(string.Format("{0}:{1}",
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss --"), sMsg));
                        sr.Flush();
                    }
                }
                catch { }
            }
        }
    }
}
