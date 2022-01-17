using System.ComponentModel.DataAnnotations;

namespace MyPlushBuddy.Api.Models
{
    /// <summary>
    /// PageTag detail with PageTagId
    /// </summary>
    public class PageTagViewModel : PageTagManipulationModel
    {
        public int PageTagId { get; set; }
    }
}
