using System.Collections.Generic;
using TaggedProducts.Domain.Entity;

namespace TaggedProducts.Web.Models
{
    public class TagListModel : BaseModel
    {
        public List<Tag> Tags { get; set; }
    }
}