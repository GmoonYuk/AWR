using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWR.Model
{
    public class ReturnValue
    {
        public string Period { get; set; }
        public int Flag { get; set; }
        public double Supply { get; set; }
        public double IrrigationDemand { get; set; }
        public double RequiredDepth_PaddyUnder { get; set; }
    }
}
