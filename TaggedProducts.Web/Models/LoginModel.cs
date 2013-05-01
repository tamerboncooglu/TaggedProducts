namespace TaggedProducts.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel : BaseModel
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
    }
}