using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.ResourceParameters
{
    public class PageTagsResourceParameter
    {
        const int maxPageSize = 20;
        public string Robots { get; set; }
        public string SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        // Here defined the default page size
        private int _pageSize = 10;
        public int PageSize 
        {
            get => _pageSize;
            // Here make sure that page size should not exceed the const maxPageSize value
            // If so, set the page size to maxPageSize
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; } = "PageRoute";
    }
}
