using AngularAPIJWT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AngularAPIJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AngularJWTContext _context;
        private readonly JWTSetting _setting;
        private readonly IRefreshTokenGenerator _tokenGenerator;
        public UserController(AngularJWTContext context,IOptions<JWTSetting> setting, IRefreshTokenGenerator tokenGenerator)
        {
            _context = context;
            _setting = setting.Value;
            _tokenGenerator = tokenGenerator;
        }


        TokenResponse Authenticate(string username, Claim[] claims)
        {
            TokenResponse tokenResponse = new TokenResponse();

            var tokenkey = Encoding.UTF8.GetBytes(_setting.securitykey);
            var tokenhandler = new JwtSecurityToken(

                claims:claims,
                expires:DateTime.Now.AddMinutes(2),
                signingCredentials:new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)

                );
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = _tokenGenerator.GenerateToken(username);

            return tokenResponse;
        }

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] usercred user)
        {
            TokenResponse response = new TokenResponse();
            var _user = _context.TblUsers.FirstOrDefault(o => o.Userid == user.username && o.Password == user.password);
            if(_user == null)
                return Unauthorized();

            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(_setting.securitykey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _user.Userid),
                    }
                ),

                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials =  new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenhandler.CreateToken(tokenDescriptor);
            string finaltoken =  tokenhandler.WriteToken(token);

            response.JWTToken = finaltoken;
            response.RefreshToken = _tokenGenerator.GenerateToken(user.username);

            return Ok(response);
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenhandler.ValidateToken(token.JWTToken, new TokenValidationParameters { 
             
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.securitykey)),  
                ValidateIssuer = false,
                ValidateAudience = false,
            
            }, out securityToken);

            var _token = securityToken as JwtSecurityToken;

            if(_token != null && !_token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return Unauthorized();
            }

            var username = principal.Identity.Name;
            var _reftable = _context.TblRefreshtokens.FirstOrDefault(o => o.UserId == username && o.RefreshToken == token.RefreshToken);
            if( _reftable == null)
            {
                return Unauthorized(); 
            }
            TokenResponse _result = Authenticate(username, principal.Claims.ToArray());
            return Ok(_result);
        }
    }
}
