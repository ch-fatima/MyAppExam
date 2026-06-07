using Core.Models;
using Core.Models.Dtos;
using Core.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.TokenHelper
{
    public class TokenGenerateHelper : ITokenGenerateHelper
    {
        private readonly InitSetting _initSetting;

        public TokenGenerateHelper(InitSetting initSetting)
        {
            _initSetting = initSetting;
        }
        
        public TokenDto GenerateTokens(User user, Guid deviceId, Guid businessKey, List<UserClaimDto> userClaims = null, List<string> UserRoleName = null)
        {
            return new TokenDto()
            {
                Accesstoken = GenerateAccessToken(user, deviceId, businessKey, userClaims, UserRoleName),
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDate = DateTime.Now.AddMinutes(Int64.Parse(_initSetting.Token.RefreshToken.ExpirationInMinutes))
            };
        }
        public string GenerateAccessToken(User user, Guid deviceId, Guid businessKey, List<UserClaimDto> userClaims = null, List<string> UserRoleName = null)
        {

            string JwtSecret = _initSetting.Token.AccessToken.Secret;
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecret);
            var Subject = new ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("PhoneNo", user.PhoneNo),
                    new System.Security.Claims.Claim("UserName", user.UserName?? string.Empty),
                    new System.Security.Claims.Claim("Email", user.Email?? string.Empty)
                });

            if (userClaims != null && userClaims.Any())
                foreach (var item in userClaims)
                {
                    Subject.AddClaim(new System.Security.Claims.Claim(item.Name, item.Value.ToString()));
                }
            if (UserRoleName != null && UserRoleName.Any())
                foreach (var item in UserRoleName)
                {
                    Subject.AddClaim(new System.Security.Claims.Claim("UserRole", item));
                }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = Subject,
                Expires = DateTime.UtcNow.AddMinutes(Int64.Parse(_initSetting.Token.AccessToken.ExpirationInMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
        private string GenerateRefreshToken()
        {
            var jwtToken = Guid.NewGuid();
            return jwtToken.ToString();
        }
    }
}
