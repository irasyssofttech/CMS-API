using System.ComponentModel.DataAnnotations;

namespace MyPlushBuddy.Api.Models
{
    public class EnquiryModel
    {        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please provide mail to contact you.")]
        [MaxLength(100)]
        public string EnquiryMail { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        //public List<IFormFile> Attachment { get; set; }
        //public string FileName { get; set; }
    }
}
