using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayz.FrameWork
{
    public class BayzLogHelper
    {
        private ILog logger;
        private static BayzLogHelper instance = new BayzLogHelper();
        private BayzLogHelper()
        {
            log4net.Config.XmlConfigurator.Configure();
            logger = log4net.LogManager.GetLogger("logger");
        }

        public static ILog Logger
        {
            get { return instance.logger; }
        }

        public static void WriteLog(string message)
        {
            BayzLogHelper.Logger.Info(message);
        }

        public static void WriteLog(Exception ex)
        {
            BayzLogHelper.Logger.Error(Trace.CurrentMethodName, ex);
        }
    }
}
