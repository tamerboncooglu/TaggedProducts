using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mutfak.Web.Models
{
    public class TagModel : BaseModel
    {
        public string Name { get; set; }
        public string UrlName { get; set; }
        public int ProductCount { get; set; }
    }
}