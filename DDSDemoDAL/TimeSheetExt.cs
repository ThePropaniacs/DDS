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
                return (this.StopTime.HasValue) 
                    ? this.StopTime.GetValueOrDefault().Subtract(this.StartTime.GetValueOrDefault())
                    : DateTime.Now.Subtract(this.StartTime.GetValueOrDefault());
            }
        }
        
    }
}
