namespace TaggedProducts.Web.Models
{
    public class TagModel : BaseModel
    {
        public string Name { get; set; }
        public string UrlName { get; set; }
        public int ProductCount { get; set; }
    }
}