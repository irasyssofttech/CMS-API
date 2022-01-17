using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            // service is scoped, created once for each request => we only need
            // to fetch the info in the constructor
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            var userIdentity = new ClaimsIdentity("Custom");
            var authType = userIdentity.AuthenticationType;
            var currentContext = _httpContextAccessor.HttpContext;
            var authType1 = currentContext.User;
            if (currentContext == null || !currentContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            UserId = currentContext
                .User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            FirstName = currentContext.User
                .Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;

            LastName = currentContext
                .User.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;

            Role = currentContext
              .User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }
    }
}

