using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWR.Model
{
    public class RequiredDepth
    {
        /// <summary>
        /// 플래그
        /// </summary>
        public int Flag { get; set; }

        /// <summary>
        /// 생장기별 구분
        /// </summary>
        public string GrowingType { get; set; }

        /// <summary>
        /// 기간시작
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 기간종료
        /// </summary>
        public string endDate { get; set; }

        /// <summary>
        /// 논바닥 밑 필요수심 (m)
        /// </summary>
        public double RequiredDepth_PaddyUnder { get; set; }

        /// <summary>
        /// 논바닥 위 필요수심(m)
        /// </summary>
        public double RequiredDepth_PaddyAbove { get; set; }

        /// <summary>
        /// 필요수심(m)
        /// </summary>
        public double RequiredDepth_Sum { get; set; }

        /// <summary>
        /// 필요면적(%)
        /// </summary>
        public double RequiredArea { get; set; }

        /// <summary>
        /// 필요수량(K㎥)
        /// </summary>
        public double RequiredQuantity { get; set; }

        /// <summary>
        /// 농업용수 공급량(㎥)
        /// </summary>
        public double SupplyQuantity { get; set; }

    }
}
