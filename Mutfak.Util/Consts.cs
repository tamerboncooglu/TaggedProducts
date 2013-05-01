using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaggedProducts.Utils
{
    public class Consts
    {
        private static CultureInfo _cultureTR;
        public static CultureInfo CultureTR
        {
            get { return _cultureTR ?? (_cultureTR = new CultureInfo("tr-TR")); }
        }

        public const string DBName = "MutfakDB";
        public const string System = "System";

        private const string Euro = "EUR";
        private const string Dolar = "USD";
        private const string Lira = "TRL";
        private const string Sterlin = "GBP";
        public static readonly string[] Currencies = new[] { Euro, Dolar, Lira, Sterlin };
    }
}
