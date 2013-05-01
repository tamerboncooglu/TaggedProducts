using System.Collections.Generic;

namespace TaggedProducts.Domain.Entity
{
    public class Contact : BaseEntity
    {
        public string Name { get; set; }
        public string PhoneOrEmail { get; set; }
        public string Message { get; set; }

        public bool IsRead { get; set; }

        public List<string> ProductUrls { get; set; }
    }
}