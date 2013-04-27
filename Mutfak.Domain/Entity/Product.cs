using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutfak.Domain.Entity
{
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string ImageUrlPrimary { get; set; }
        public string VideoUrlPrimary { get; set; }
        IList<string> Categories { get; set; }
        public string Category { get; set; }
        public bool IsSold { get; set; }
    }
}
