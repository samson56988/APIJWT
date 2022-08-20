using AngularAPIJWT.Models;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace AngularAPIJWT
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly AngularJWTContext _context;

        public RefreshTokenGenerator(AngularJWTContext context)
        {
            _context = context;
        }

        public string GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using(var randomnumbergenerator =  RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string RefreshToken =  Convert.ToBase64String(randomnumber);

                var _user = _context.TblRefreshtokens.FirstOrDefault(o => o.UserId == username);
                if(_user != null)
                {
                    _user.RefreshToken = RefreshToken;
                    _context.SaveChanges();
                }
                else
                {
                    TblRefreshtoken tbl = new TblRefreshtoken()
                    {
                        UserId = username,
                        TokenId = new Random().Next().ToString(),
                        RefreshToken = RefreshToken,
                        IsActive = true 
                    };
                }

                return RefreshToken;
            }
        }
    }
}
