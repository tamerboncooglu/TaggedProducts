using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutfak.Util
{
    public class Consts
    {
        private static CultureInfo _cultureTR;
        public static CultureInfo CultureTR
        {
            get { return _cultureTR ?? (_cultureTR = new CultureInfo("tr-TR")); }
        }

        public const string DBName = "MutfakDB";
    }
}
