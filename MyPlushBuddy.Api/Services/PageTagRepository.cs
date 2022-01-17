using Microsoft.EntityFrameworkCore;
using MyPlushBuddy.Api.Contexts;
using MyPlushBuddy.Api.Entities;
using MyPlushBuddy.Api.Helpers;
using MyPlushBuddy.Api.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Services
{
    public class PageTagRepository : IPageTagRepository, IDisposable
    {
        private MyPlushBuddyContext dbContext;
        private readonly IPropertyMappingService propertyMappingService;

        public PageTagRepository(MyPlushBuddyContext dbContext,
            IPropertyMappingService propertyMappingService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public async Task<PageTag> GetPageTagAsync(int pageTagId)
        {
            return await dbContext.PageTags.Where(p => p.PageTagId == pageTagId).FirstOrDefaultAsync();
        }

        public async Task<PagedList<PageTag>> GetPageTagsAsync(PageTagsResourceParameter pageTagsResourceParam)
        {
            if (pageTagsResourceParam == null)
            {
                throw new ArgumentNullException(nameof(pageTagsResourceParam));
            }

            //Deferred execution started by creation IQueryable object page Entity
            var pageTagsCollection = dbContext.PageTags as IQueryable<PageTag>;

            // Filtering implementation
            if (!string.IsNullOrWhiteSpace(pageTagsResourceParam.Robots))
            {
                var robots = pageTagsResourceParam.Robots.Trim();
                pageTagsCollection = pageTagsCollection.Where(p => p.Robots == robots);
            }

            // Searching implementation
            if (!string.IsNullOrWhiteSpace(pageTagsResourceParam.SearchQuery))
            {
                var searchQuery = pageTagsResourceParam.SearchQuery.Trim();
                pageTagsCollection = pageTagsCollection.Where(p => p.PageTitle.Contains(searchQuery)
                || p.Ogdescription.Contains(searchQuery)
                || p.MetaNameDescription.Contains(searchQuery));

            }

            if (!string.IsNullOrWhiteSpace(pageTagsResourceParam.OrderBy))
            {               
                // get property mapping dictionary
                var pageTagPropertyMappingDictionary =
                    propertyMappingService.GetPropertyMapping<Models.PageTagViewModel, Entities.PageTag>();

                pageTagsCollection = pageTagsCollection.ApplySort(pageTagsResourceParam.OrderBy, 
                    pageTagPropertyMappingDictionary);
            }

            // Deferred Execution here. As query executed only when we are calling ToList() here.
            return await PagedList<PageTag>.Create(pageTagsCollection, 
                pageTagsResourceParam.PageNumber, 
                pageTagsResourceParam.PageSize);

        }

        public async Task<bool> PageTagExistAsync(int pageTagId)
        {
            return await dbContext.PageTags.AnyAsync(p => p.PageTagId == pageTagId);
        }

        public void AddPageTag(PageTag pageTag)
        {
            if (pageTag == null)
            {
                throw new ArgumentNullException(nameof(pageTag));
            }
            dbContext.PageTags.Add(pageTag);
        }

        public void UpdatePageTag(PageTag pageTag)
        {
            dbContext.PageTags.Update(pageTag);
        }

        public async Task<bool> SaveAsync()
        {
            return (await dbContext.SaveChangesAsync() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                    dbContext = null;
                }
            }
        }
    }
}
