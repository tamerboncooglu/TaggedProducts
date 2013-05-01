using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaggedProducts.Domain.Entity
{
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HtmlDescription { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        public string ImageUrlPrimary { get; set; }
        public string ImageUrlPrimarySmall { get; set; }
        public string ImageUrlPrimaryBig { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<string> ImageUrlBigs { get; set; }
        public List<string> ImageUrlSmalls { get; set; }

        public string VideoUrl { get; set; }
        public IList<string> Tags { get; set; }
        public bool IsSold { get; set; }
    }
}
