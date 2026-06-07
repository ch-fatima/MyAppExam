using Core.Models.Dtos;
using System;
using System.Collections.Generic;

namespace Infrastructure.TokenHelper
{
    public interface ICurrentUserHelper
    {
        UserTokenDto GetUserFromToken();
    }
}
