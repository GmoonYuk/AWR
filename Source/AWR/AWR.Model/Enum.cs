using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWR.Model
{
    public enum GrowingType
    {
        /// <summary>
        /// 묘대기
        /// </summary>
        Step1 = 1,
        /// <summary>
        /// 이양기,착근기
        /// </summary>
        Step2 = 2,
        /// <summary>
        /// 분얼기
        /// </summary>
        Step3 = 3,
        /// <summary>
        /// 생식기
        /// </summary>
        Step4 = 4,
        /// <summary>
        /// 휴농
        /// </summary>
        Step5 = 5
    }
}
