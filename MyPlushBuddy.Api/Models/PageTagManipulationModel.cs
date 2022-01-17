using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Models
{    
    public abstract class PageTagManipulationModel: IValidatableObject
    {
        /// <summary>
        /// PageRoute is also unique identifier to identify the pagetag details
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string PageRoute { get; set; }

        /// <summary>
        /// PageTitle is used to show the html page title at top.
        /// </summary>
        [Required]
        [MaxLength(5000)]        
        public string PageTitle { get; set; }

        /// <summary>
        /// Accept only 'WebApplication' or 'Article' value
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Robots { get; set; }

        /// <summary>
        /// Open Graph title for page
        /// </summary>
        [MaxLength(5000)]
        public string Ogtitle { get; set; }

        /// <summary>
        /// Open Graph type
        /// </summary>
        [MaxLength(20)]
        public string Ogtype { get; set; }

        /// <summary>
        /// Open Graph Url
        /// </summary>
        [MaxLength(5000)]
        public string Ogurl { get; set; }

        /// <summary>
        /// Open Graph site name, Set 'My Plush Buddy' on it
        /// </summary>
        [MaxLength(5000)]
        public string OgsiteName { get; set; }

        /// <summary>
        /// Open Graph Article Publisher. Set 'My Plush Buddy' on it
        /// </summary>
        [MaxLength(5000)]
        public string OgarcticlePublisher { get; set; }

        /// <summary>
        /// Open Graph description
        /// </summary>
        [MaxLength(5000)]
        public string Ogdescription { get; set; }

        /// <summary>
        /// Open Graph image used for Sharing on facebook and other popular sharing application
        /// </summary>
        [MaxLength(5000)]
        public string Ogimage { get; set; }

        /// <summary>
        /// Set the supported Language type. Set en_US on it
        /// </summary>
        [MaxLength(10)]
        public string Oglocal { get; set; }

        /// <summary>
        /// Define TwitterCard type, set 'summary_large_image' on it
        /// </summary>
        [MaxLength(5000)]
        public string TwitterCard { get; set; }

        /// <summary>
        /// Twitter Creator will always be @myplushbuddy
        /// </summary>
        [MaxLength(5000)]
        public string TwitterCreator { get; set; }

        /// <summary>
        /// Twitter Site will always be @myplushbuddy
        /// </summary>
        [MaxLength(5000)]
        public string TwitterSite { get; set; }

        /// <summary>
        /// Twitter image will used while user will share the page on twitter.
        /// </summary>
        [MaxLength(5000)]
        public string TwitterImage { get; set; }

        /// <summary>
        /// Page Key Words for SEO
        /// </summary>
        [MaxLength(5000)]
        public string PageKeywords { get; set; }

        /// <summary>
        /// Meta Name description will give details about the web page and this text will used in SEO.
        /// </summary>
        [MaxLength(5000)]
        public string MetaNameDescription { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var robots = new List<string> { "index, follow", "noindex" };
            var ogtypes = new List<string> { "Article", "Website" };

            if (!robots.Contains(Robots.ToLower()))
            {
                yield return new ValidationResult(
                    "Robots should contain either 'index, follow' or 'noindex'",
                    new[] { "Robots" });
            }
            else if (!ogtypes.Contains(Ogtype))
            {
                yield return new ValidationResult(
                    "OGType should contain either 'Article' or 'Website'. It is case sensitive",
                    new[] { "Robots" });
            }
        }
    }
}
