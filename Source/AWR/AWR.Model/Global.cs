using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWR.Model
{
    public class Global
    {
        private static Global instance = null;

        //변수
        public List<AWR_InputTemplate> listAWRInput { get; set; }
        public List<RequiredDepth> listAWRSet { get; set; }

        public double BenefitArea { get; set; }
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
