namespace Mutfak.Web.Models
{
    public class ContactModel : BaseModel
    {
        public string Name { get; set; }
        public string PhoneOrEmail { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}