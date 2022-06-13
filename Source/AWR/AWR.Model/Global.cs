using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWR.Model
{
    /// <summary>
    /// 전역변수 클래스
    /// </summary>
    public class Global
    {
        private static Global instance = null;

        //변수
        /// <summary>
        /// 일자, 강우, 증발산 입력자료 리스트
        /// </summary>
        public List<AWR_InputTemplate> listAWRInput { get; set; }

        /// <summary>
        /// 연산 설정값 리스트
        /// </summary>
        public List<RequiredDepth> listAWRSet { get; set; }

        /// <summary>
        /// 수해면적
        /// </summary>
        public double BenefitArea { get; set; }

        /// <summary>
        /// 초기수위
        /// </summary>
        public double InitVolume { get; set; }

        static Global()
        {
            instance = new Global();
        }

        private Global()
        {
            this.listAWRInput = new List<AWR_InputTemplate>();
            this.listAWRSet = new List<RequiredDepth>();
        }

        public static Global GetInstance()
        {
            return instance;
        }
    }
}
