using IdentityandJwt.Api2.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.WebModel.Login
{
    public class UserHandler
    {
        public IConfiguration Configuration { get; set; }
        public UserHandler(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //Token üretecek metot.
        public Token CreateAccessToken(AppUser user)
        {
            Token tokenInstance = new Token();

            //Security  Key'in simetriğini alıyoruz.
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:SecurityKey"]));

            //Şifrelenmiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Oluşturulacak token ayarlarını veriyoruz.
            tokenInstance.AccessTokenExpiration = DateTime.Now.AddMinutes(2);
            tokenInstance.RefreshTokenExpiration = DateTime.Now.AddMinutes(4);
            var claims = new List<Claim>();
            claims.Add(new Claim("Username",user.UserName));
            claims.Add(new Claim("Mail",user.Email));
            claims.Add(new Claim("id",user.Id));
            claims.Add(new Claim("firstname", user.FirstName));
            claims.Add(new Claim("lastname",user.LastName));

            JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: Configuration["Token:Issuer"],
            audience: Configuration["Token:Audience"],
            expires: tokenInstance.AccessTokenExpiration,//Token süresini 5 dk olarak belirliyorum
            notBefore: DateTime.Now,//Token üretildikten ne kadar süre sonra devreye girsin ayarlıyouz.
            signingCredentials: signingCredentials,
            claims:claims
            );

            //Token oluşturucu sınıfında bir örnek alıyoruz.
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            //Token üretiyoruz.
            tokenInstance.AccessToken = tokenHandler.WriteToken(securityToken);

            //Refresh Token üretiyoruz.
            tokenInstance.RefreshToken = CreateRefreshToken();
            return tokenInstance;
        }

        //Refresh Token üretecek metot.
        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
                return Convert.ToBase64String(number);
            }
        }
    }
}
