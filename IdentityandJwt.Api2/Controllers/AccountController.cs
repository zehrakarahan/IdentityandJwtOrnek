using IdentityandJwt.Api2.Model;
using IdentityandJwt.Api2.WebModel;
using IdentityandJwt.Api2.WebModel.Login;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[EnableCors("_myAllowSpecificOrigins")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        public readonly IHttpContextAccessor _httpContextAccessor;
        public ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
       IConfiguration configuration,SignInManager<AppUser> signInManager,
        IHttpContextAccessor httpContextAccessor, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        [HttpGet]
        [Route("Login")]
        public IActionResult Index()
        {
            return Ok(_userManager.Users.ToList());
        }
       
        [HttpPost]
        [Route("Login")]
       
        public async Task<IActionResult> Login([FromBody]User usermodel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(usermodel.UserName);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, usermodel.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (user.AccessTokenExpiration == null || _httpContextAccessor.HttpContext.Request.Cookies["AccessToken"] == null ||
                            _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"] == null || user.AccessToken!=null || user.RefreshToken!=null)
                        {
                            UserHandler tokenHandler = new UserHandler(_configuration);
                            Token token = tokenHandler.CreateAccessToken(user);

                            //Refresh token Users tablosuna işleniyor.
                            token.UserName = usermodel.UserName;
                            IList<string> rolename = await _userManager.GetRolesAsync(user);
                            token.RoleName = rolename.FirstOrDefault();
                            user.AccessToken = token.AccessToken;
                            user.RefreshToken = token.RefreshToken;
                            user.AccessTokenExpiration = token.AccessTokenExpiration.AddMinutes(2);
                            user.RefreshTokenExpiration =user.AccessTokenExpiration.Value.AddMinutes(2);
                            _httpContextAccessor.HttpContext.Response.Cookies.Append
                                  ("AccessToken", token.AccessToken, new CookieOptions
                                  {
                                      HttpOnly = true,
                                      SameSite = SameSiteMode.Lax,
                                      Expires = token.AccessTokenExpiration

                                  });

                            _httpContextAccessor.HttpContext.Response.Cookies.Append
                                ("RefreshToken", token.RefreshToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    SameSite = SameSiteMode.Lax,
                                    Expires = token.RefreshTokenExpiration

                                });
                         
                            _logger.LogInformation("AccountController Login giriş yapmaya çalışan kullanıcı başarılı giriş yapmıştır {date}", DateTime.UtcNow);
                            await _userManager.UpdateAsync(user);

                            return Ok(token);
                        }
                        else if(user.AccessTokenExpiration <DateTime.Now)
                        {
                            // return Forbid();//403 hatası vermesi için kullanılıyor.Bunu kullanma nedeni accestoken süresi bitti refresh token uretmesi için kullanılıyor
                            _logger.LogInformation("AccountController Login giriş yapmaya çalışan kullanıcının token suresi bitmiştir {date}", DateTime.UtcNow);
                            return StatusCode(403);
                         
                        }
                        else
                        {
                            _logger.LogInformation("AccountController Login giriş yapmaya çalışan kullanıcı bulunamadı. {date}", DateTime.UtcNow);
                            return Unauthorized(new FeedBackMessage {
                               Message="Kullanıcının Yetkisi Yoktur",
                               StatusCode=401.ToString()
                            
                            });
                        }
                    }
                    else
                    {
                        return Unauthorized(new FeedBackMessage {
                        Message="Kullanıcı Adı veya Şifre yanlıştır",
                        StatusCode=401.ToString()
                        
                        });
                    }
                }
                else
                {
                    return Unauthorized(new FeedBackMessage {
                    Message="Böyle Bir Kullanıcı Yoktur.",
                    StatusCode=401.ToString()
                    
                    });
                }
               
            }
            else
            {
                return BadRequest(new FeedBackMessage { 
                Message="Kullanıcı Adı veya Şifre boş geçilemez.",
                StatusCode=401.ToString()
                
                });
            }
        
        }
        //[HttpGet]
        //public JwtSecurityToken AccessTokenCreate(UserAccesToken usermodel)
        //{

        //    if (usermodel!=null)
        //    {
        //        var claims = new[]
        //        {
        //                        new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
        //                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        //                        new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
        //                        new Claim("Id",usermodel.Id.ToString()),
        //                        new Claim("UserName",usermodel.UserName),
        //                        new Claim("Password",usermodel.Password)

        //                };
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //        var token = new JwtSecurityToken(
        //            _configuration["Jwt:Issuer"],
        //            _configuration["Jwt:Audience"],
        //            claims,
        //            expires: DateTime.Now.AddHours(3),
        //            signingCredentials: signIn

        //           );
        //        return token;
        //    }
        //    else
        //    {
        //        return null;
        //    }
              
        //}
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> SignUp([FromBody]UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser();
                appUser.UserName = userViewModel.UserName;
                appUser.Email = userViewModel.Email;
                appUser.PhoneNumber = userViewModel.PhoneNumber;
                IdentityResult result = await _userManager.CreateAsync(appUser, userViewModel.Password);
                if (result.Succeeded)
                {
                    return Ok(userViewModel);
                }
                else
                {
                    return NotFound(result.Errors.ToList());
                }

            }
            else
            {
                return BadRequest();
            }
       
        }
        [HttpPost]
        [Route("RefreshTokenLogin")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody]RefreshTokenModel refreshToken)
        {

            if (refreshToken.RefreshToken!="" || refreshToken.RefreshToken!=null)
            {
                AppUser appUser = _userManager.Users.FirstOrDefault(x => x.RefreshToken == refreshToken.RefreshToken);//bu refresh tokena sahip olan kullanıcıya bakıyoruz
                if (appUser == null || appUser?.RefreshTokenExpiration < DateTime.Now) 
                {
                    // return Forbid();//403 hatası vermesi için kullanılıyor.Bunu kullanma nedeni accestoken süresi bitti refresh token uretmesi için kullanılıyor
                    return StatusCode(403);
                }
                else if (appUser!=null && appUser?.RefreshTokenExpiration>DateTime.Now)
                {
                    UserHandler tokenHandler = new UserHandler(_configuration);
                    Token token = tokenHandler.CreateAccessToken(appUser);
                    appUser.RefreshToken = token.RefreshToken;
                    appUser.AccessTokenExpiration = token.AccessTokenExpiration.AddMinutes(2);
                    appUser.RefreshTokenExpiration = appUser.AccessTokenExpiration.Value.AddMinutes(2);
                    _httpContextAccessor.HttpContext.Response.Cookies.Append
                                  ("AccessToken", token.AccessToken, new CookieOptions
                                  {
                                      HttpOnly = true,
                                      SameSite = SameSiteMode.Lax,
                                      Expires = token.AccessTokenExpiration

                                  });

                    _httpContextAccessor.HttpContext.Response.Cookies.Append
                        ("RefreshToken", token.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = token.RefreshTokenExpiration

                        });
                    await _userManager.UpdateAsync(appUser);
                    return Ok(token);
                }
             
                else
                {
                    return Unauthorized(new FeedBackMessage { 
                    Message="RefreshToken bilgisi yanlış",
                    StatusCode=401.ToString()
                    
                    
                    });
                }
              
            }
            else
            {
                return Unauthorized(new FeedBackMessage
                {
                    Message = "RefreshToken bilgisi yanlış",
                    StatusCode = 401.ToString()


                });
            }

        }


    }
}
