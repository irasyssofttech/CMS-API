using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Entities
{
    public class PageTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PageTagId { get; set; }
        [Required]
        [MaxLength(500)]
        public string PageRoute { get; set; }
        [Required]
        [MaxLength(5000)]
        public string PageTitle { get; set; }
        [Required]
        [MaxLength(100)]
        public string Robots { get; set; }
        [MaxLength(5000)]
        public string Ogtitle { get; set; }
        [MaxLength(20)]
        public string Ogtype { get; set; }
        [MaxLength(5000)]
        public string Ogurl { get; set; }
        [MaxLength(5000)]
        public string OgsiteName { get; set; }
        [MaxLength(5000)]
        public string OgarcticlePublisher { get; set; }
        [MaxLength(5000)]
        public string Ogdescription { get; set; }
        [MaxLength(5000)]
        public string Ogimage { get; set; }
        [MaxLength(10)]
        public string Oglocal { get; set; }
        [MaxLength(5000)]
        public string TwitterCard { get; set; }
        [MaxLength(5000)]
        public string TwitterCreator { get; set; }
        [MaxLength(5000)]
        public string TwitterSite { get; set; }
        [MaxLength(5000)]
        public string TwitterImage { get; set; }
        [MaxLength(5000)]
        public string PageKeywords { get; set; }
        [MaxLength(5000)]
        public string MetaNameDescription { get; set; }
    }
}
