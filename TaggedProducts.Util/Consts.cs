using System.Globalization;

namespace TaggedProducts.Utils
{
    public static class Consts
    {
        private static CultureInfo _cultureTR;
        public static CultureInfo CultureTR
        {
            get { return _cultureTR ?? (_cultureTR = new CultureInfo("tr-TR")); }
        }

        public const string DBName = "TaggedProductsDB";
        public const string System = "System";

        private const string Euro = "EUR";
        private const string Dolar = "USD";
        private const string Lira = "TRL";
        private const string Sterlin = "GBP";
        public static readonly string[] Currencies = new[] { Euro, Dolar, Lira, Sterlin };
    }
}
