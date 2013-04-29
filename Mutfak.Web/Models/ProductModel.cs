using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mutfak.Web.Models
{
    public class ProductModel : BaseModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HtmlDescription { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        public HttpPostedFileBase[] Images { get; set; }
        public List<string> ImageUrls { get; set; }
        public string ImageUrlPrimary { get; set; }
        public bool IsSold { get; set; }
        public ContactModel ContactModel { get; set; }
        public string VideoUrl { get; set; }
        public IList<string> Tags { get; set; }
    }
}