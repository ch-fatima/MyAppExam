using Core.Models.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.TokenHelper
{
    public class CurrentUserHelper : ICurrentUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserTokenDto GetUserFromToken()
        {
            var cp = _httpContextAccessor.HttpContext.User;

            var UserName = cp.FindFirst(c => c.Type == "UserName").Value;

            var Email = cp.FindFirst(c => c.Type == "Email").Value;

            var PhoneNo = cp.FindFirst(c => c.Type == "PhoneNo").Value;

            var userRoles = cp.FindAll(c => string.Equals(c.Type.ToLower(), "userrole")).Select(c => c.Value).ToList();

            List<string> listOfRoles = cp.FindAll(c => c.Type.ToLower().Contains("role")).Select(c => c.Value).ToList();


            return new UserTokenDto()
            {
                UserName = UserName,
                Email =Email,    
                PhoneNo = PhoneNo,
                Roles = listOfRoles,
                Claims = cp.Claims.ToList(),
                UserRoles = userRoles
            };
        }

    }
}
