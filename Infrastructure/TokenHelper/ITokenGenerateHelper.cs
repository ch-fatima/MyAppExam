using Core.Models.Dtos;
using Core.Models.Entities;
using System;
using System.Collections.Generic;

namespace Infrastructure.TokenHelper
{
    public interface ITokenGenerateHelper
    {
        TokenDto GenerateTokens(User user, Guid deviceId, Guid businessKey,
            List<UserClaimDto> userClaims = null, List<string> roles = null);

        string GenerateAccessToken(User user, Guid deviceId, Guid businessKey,
            List<UserClaimDto> userClaims = null, List<string> roles = null);
    }
}
