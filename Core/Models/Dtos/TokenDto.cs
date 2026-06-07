using System;

namespace Core.Models.Dtos
{
    public class TokenDto
    {
        public string Accesstoken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
    }
}
