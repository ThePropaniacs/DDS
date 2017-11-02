using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDSDemoDAL
{
    public partial class TimeSheet
    {
        public TimeSpan ElapsedTime
        {
            get
            {
                return (this.stopTime.HasValue) 
                    ? this.stopTime.GetValueOrDefault().Subtract(this.startTime.GetValueOrDefault())
                    : DateTimeOffset.Now.Subtract(this.startTime.GetValueOrDefault());
            }
        }
    }
}
