using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mutfak.Domain.Entity;

namespace Mutfak.Web.Models
{
    public class TagListModel : BaseModel
    {
        public List<Tag> Tags { get; set; }
    }
}