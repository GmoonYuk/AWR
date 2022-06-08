using AWR.Model;
using Bayz.FrameWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWR.Control
{
    public static class BayzFileIO
    {
        public static List<AWR_InputTemplate> ReadAWR_InputTemplate(string filePath)
        {
            try
            {
                List<AWR_InputTemplate> listInput = new List<AWR_InputTemplate>();

                using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
                {

                }

                return listInput;
            }
            catch (Exception ex)
            {
                BayzLogHelper.WriteLog(ex.StackTrace.ToString());
                BayzLogHelper.WriteLog(ex.Message.ToString());

                return null;
            }
            
        }
    }
}
