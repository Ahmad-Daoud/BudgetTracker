using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Utilities
{
    public static class DateHelper
    {
        public static string ToDateTime(this DateTime d)
        {
            return d.ToString();
        }
    }
}
