using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWR.Model;

namespace AWR.Control
{
    public static class BayzCalc
    {
        public static void CalcRequiredQuantity(List<RequiredDepth> listAWRSet, double benefitArea)
        {
            foreach (RequiredDepth item in listAWRSet)
            {
                if (item.Flag != 5)
                {
                    item.RequiredQuantity = benefitArea * item.RequiredDepth_Sum * item.RequiredArea / 100 / 1000;
                }
                
            }
        }

        public static List<AWR_Result> CalcAWR(List<AWR_InputTemplate> listAWRInput, List<RequiredDepth> listAWRSet, double benefitArea, double initVolume)
        {
            List<AWR_Result> listResult = new List<AWR_Result>();

            bool bStart = false;
            double beforeVolume = 0.0;

            foreach (AWR_InputTemplate input in listAWRInput)
            {
                AWR_Result addData = new AWR_Result();

                addData.curDate = input.curDate;

                if (bStart == false)
                {
                    addData.BeginningStorage_Km = initVolume;
                    bStart = true;
                }
                else
                {
                    addData.BeginningStorage_Km = beforeVolume;
                }

                addData.BeginningStorage_mm = addData.BeginningStorage_Km / benefitArea;

                
                ReturnValue rv = new ReturnValue();
                rv = CalcReturnValue(input.curDate, listAWRSet);

                addData.Period = rv.Period;
                addData.Flag = rv.Flag;
                addData.IrrigationInflow_ms = rv.Supply;

                addData.IrrigationInflow_km = Math.Round(addData.IrrigationInflow_ms * 24 * 3600 / 1000, 0);
                addData.IrrigationInflow_mm = (addData.IrrigationInflow_ms * 24 * 3600 / 1000) * 1000 * 1000 / benefitArea;

                addData.Rainfall_mm = input.Rainfall;
                addData.Rainfall_km = input.Rainfall * benefitArea / 1000;

                addData.Evaporation_mm = input.Evaporation;
                addData.Evaporation_km = input.Evaporation * benefitArea / 1000;

                addData.NetEvaporation_mm = CalcNetEvaporation(addData.Rainfall_mm, addData.Evaporation_mm, rv.RequiredDepth_PaddyUnder);
                addData.NetEvaporation_km = addData.NetEvaporation_mm * benefitArea / 1000;

                addData.IrrigationDemand_km = rv.IrrigationDemand;

                addData.EndingStorage_km = CalcEndingStorage(addData.IrrigationDemand_km, addData.BeginningStorage_Km
                    , addData.IrrigationInflow_km, addData.Rainfall_km, addData.NetEvaporation_km);

                addData.EndingStorage_mm = addData.EndingStorage_km * 1000 / benefitArea;

                DateTime curDate = new DateTime(2000, 5, 2);
                if (addData.curDate == curDate)
                {
                    int kkk = 0;
                }
                listResult.Add(addData);
            }

            return listResult;
        }

        private static double CalcEndingStorage(double irrigationDemand, double beginningStorage, double irrigationInflow, double rainfall, double netEvaporation)
        {
            double EndingStorage = 0.0;

            if (irrigationDemand > beginningStorage)
            {
                if ((beginningStorage + irrigationInflow + rainfall - netEvaporation) > irrigationDemand)
                {
                    EndingStorage = irrigationDemand;
                }
                else
                {
                    EndingStorage = beginningStorage + irrigationInflow + rainfall - netEvaporation;
                }                
            }
            else
            {
                EndingStorage = beginningStorage;
            }

            return EndingStorage;
        }

        private static double CalcNetEvaporation(double rainfall, double evaporation, double requiredDepth_PaddyUnder)
        {
            double NetEvaporation = 0.0;

            if (rainfall >= evaporation)
            {
                NetEvaporation = 0.0;
            }
            else
            {
                if ((evaporation - rainfall) > (requiredDepth_PaddyUnder * 1000))
                {
                    NetEvaporation = requiredDepth_PaddyUnder * 1000;
                }
                else
                {
                    NetEvaporation = evaporation - rainfall;
                }
            }

            return NetEvaporation;
        }

        private static ReturnValue CalcReturnValue(DateTime curDate, List<RequiredDepth> listAWRSet)
        {
            ReturnValue rv = new ReturnValue();

            int year = curDate.Year;

            foreach (RequiredDepth item in listAWRSet)
            {
                if (item.Flag != 5)
                {
                    string[] spliteDate = item.StartDate.Split('/');
                    int month = int.Parse(spliteDate[0]);
                    int day = int.Parse(spliteDate[1]);
                    DateTime startDate = new DateTime(year, month, day);

                    spliteDate = item.endDate.Split('/');
                    month = int.Parse(spliteDate[0]);
                    day = int.Parse(spliteDate[1]);
                    DateTime endDate = new DateTime(year, month, day);

                    if (curDate >= startDate && curDate <= endDate)
                    {
                        rv.Period = item.GrowingType;
                        rv.Flag = item.Flag;
                        rv.Supply = item.SupplyQuantity;
                        rv.IrrigationDemand = item.RequiredQuantity;
                        rv.RequiredDepth_PaddyUnder = item.RequiredDepth_PaddyUnder;
                        break;
                    }
                }
                else
                {
                    rv.Period = item.GrowingType;
                    rv.Flag = item.Flag;
                    rv.Supply = item.SupplyQuantity;
                    rv.IrrigationDemand = item.RequiredQuantity;
                    rv.RequiredDepth_PaddyUnder = item.RequiredDepth_PaddyUnder;
                    break;
                }

            }

            return rv;
        }

        private static int CalcFlag(DateTime curDate, List<RequiredDepth> listAWRSet)
        {
            int rtnFlag = 0;

            int year = curDate.Year;

            foreach (RequiredDepth item in listAWRSet)
            {
                if (item.Flag != 5)
                {
                    string[] spliteDate = item.StartDate.Split('/');
                    int month = int.Parse(spliteDate[0]);
                    int day = int.Parse(spliteDate[1]);
                    DateTime startDate = new DateTime(year, month, day);

                    spliteDate = item.endDate.Split('/');
                    month = int.Parse(spliteDate[0]);
                    day = int.Parse(spliteDate[1]);
                    DateTime endDate = new DateTime(year, month, day);

                    if (curDate >= startDate && curDate <= endDate)
                    {
                        rtnFlag = item.Flag;
                        break;
                    }
                }
                else
                {
                    rtnFlag = item.Flag;
                    break;
                }
            }

            return rtnFlag;
        }
    }
}
