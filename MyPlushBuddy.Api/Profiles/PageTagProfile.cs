using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Profiles
{
    public class PageTagProfile: Profile
    {
        public PageTagProfile()
        {
            CreateMap<Entities.PageTag, Models.PageTagViewModel>();
            CreateMap<Entities.PageTag, Models.PageTagCreateModel>();
            CreateMap<Models.PageTagCreateModel, Entities.PageTag>();
            CreateMap<Entities.PageTag, Models.PageTagUpdateModel>();
            CreateMap<Models.PageTagUpdateModel, Entities.PageTag>();
        }
    }
}
