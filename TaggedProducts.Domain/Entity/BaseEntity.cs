using System;
using MongoDB.Bson;

namespace TaggedProducts.Domain.Entity
{
    public class BaseEntity
    {
        public BsonObjectId Id { get; set; }

        public string IdStr
        {
            get
            {
                return Id.ToString();
            }
        }

        public string AdminComment { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}