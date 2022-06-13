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
        /// <summary>
        /// 강우, 증발산 csv 형식 입력자료 읽기 함수
        /// </summary>
        /// <param name="filePath">파일경로</param>
        /// <returns>입력자료 리스트</returns>
        public static List<AWR_InputTemplate> ReadAWR_InputTemplate(string filePath)
        {
            try
            {
                List<AWR_InputTemplate> listInput = new List<AWR_InputTemplate>();

                using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
                {
                    string strline = string.Empty;
                    strline = sr.ReadLine();

                    while (sr.Peek() > 0)
                    {
                        strline = sr.ReadLine();
                        string[] vals = strline.Split(new char[] { ',' });

                        if (vals.Length == 4)
                        {
                            AWR_InputTemplate addData = new AWR_InputTemplate();

                            int year = int.Parse(vals[0]);

                            string[] Date = vals[1].Split('/');
                            int month = int.Parse(Date[0]);
                            int day = int.Parse(Date[1]);

                            addData.curDate = new DateTime(year, month, day);

                            addData.Rainfall = double.Parse(vals[2]);
                            addData.Evaporation = double.Parse(vals[3]);

                            listInput.Add(addData);
                        }
                    }
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

        /// <summary>
        /// 농업용수 계산 설정파일 읽기함수
        /// </summary>
        /// <param name="filePath">파일경로</param>
        /// <returns>설정파일 리스트</returns>
        public static List<RequiredDepth> ReadAWR_Set(string filePath)
        {
            try
            {
                List<RequiredDepth> listSet = new List<RequiredDepth>();

                using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
                {
                    string strline = string.Empty;
                    strline = sr.ReadLine();

                    while (sr.Peek() > 0)
                    {
                        strline = sr.ReadLine();
                        string[] vals = strline.Split(new char[] { ',' });

                        RequiredDepth addData = new RequiredDepth();

                        addData.Flag = int.Parse(vals[0]);
                        addData.GrowingType = vals[1].Trim();

                        if (vals[2] != "")
                        {
                            addData.StartDate = vals[2].Trim();
                        }

                        if (vals[3] != "")
                        {
                            addData.endDate = vals[3].Trim();
                        }
                       
                        addData.RequiredDepth_PaddyUnder = double.Parse(vals[4]);
                        addData.RequiredDepth_PaddyAbove = double.Parse(vals[5]);
                        addData.RequiredDepth_Sum = double.Parse(vals[6]);
                        addData.RequiredArea = double.Parse(vals[7]);

                        if (vals[8] != "")
                        {
                            addData.SupplyQuantity = double.Parse(vals[8]);
                        }                        

                        listSet.Add(addData);
                       
                    }
                }

                return listSet;
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
