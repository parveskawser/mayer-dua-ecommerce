using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Framework
{
    public class ConversionBlock
    {
        public static string ConvertStringToDouble(string number)
        {
            string convertnumber = "0.00";
            if (!string.IsNullOrWhiteSpace(number))
            {
                convertnumber = string.Format("{0:#,##0.00}", Convert.ToDouble(number));
                if (string.IsNullOrWhiteSpace(convertnumber))
                {
                    convertnumber = "0.00";
                }
            }
            return convertnumber;
        }
    }
}
