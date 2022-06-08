using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWR.Model
{
    public class AWR_Result
    {
        /// <summary>
        /// 일자
        /// </summary>
        public DateTime curDate { get; set; }

        /// <summary>
        /// 시기
        /// </summary>
        public string Period { get; set; }

        public int Flag { get; set; }
        public double BeginningStorage_Km { get; set; }
        public double BeginningStorage_mm { get; set; }
        public double IrrigationInflow_ms { get; set; }
        public double IrrigationInflow_km { get; set; }
        public double IrrigationInflow_mm { get; set; }
        public double Rainfall_mm { get; set; }
        public double Rainfall_km { get; set; }
        public double Evaporation_mm { get; set; }
        public double Evaporation_km { get; set; }
        public double NetEvaporation_mm { get; set; }
        public double NetEvaporation_km { get; set; }
        public double IrrigationDemand_km { get; set; }
        public double EndingStorage_km { get; set; }
        public double EndingStorage_mm { get; set; }
        public double Outflow_km { get; set; }
        public double Outflow_ms { get; set; }
        public double RateofReturn { get; set; }
    }
}
