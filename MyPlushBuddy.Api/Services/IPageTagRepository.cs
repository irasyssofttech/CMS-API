using MyPlushBuddy.Api.Entities;
using MyPlushBuddy.Api.Helpers;
using MyPlushBuddy.Api.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Services
{
    public interface IPageTagRepository
    {
        Task<PageTag> GetPageTagAsync(int pageTagId);

        Task<PagedList<PageTag>> GetPageTagsAsync(PageTagsResourceParameter pageTagsResourceParam);
        
        Task<bool> PageTagExistAsync(int pageTagId);
        
        void AddPageTag(PageTag pageTag);

        void UpdatePageTag(PageTag pageTag);

        Task<bool> SaveAsync();

    }
}
