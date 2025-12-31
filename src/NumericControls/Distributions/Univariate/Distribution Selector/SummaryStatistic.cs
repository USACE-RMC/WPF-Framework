using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericControls.Distributions.Univariate
{
    /// <summary>
    /// A class to define the distribution summary statistics.
    /// </summary>
    public class SummaryStatistic
    {
        public SummaryStatistic(string statName, string distValue, string dataValue)
        {
            StatName = statName;
            DistStat = distValue;
            DataStat = dataValue;
        }

        public string StatName { get; private set; }
        public string DistStat { get; set; }
        public string DataStat { get; set; }
    }
}
