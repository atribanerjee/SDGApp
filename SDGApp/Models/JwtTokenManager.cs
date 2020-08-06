using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;

namespace SDGApp.Models
{
    public class JwtTokenManager
    {
        //Secret Key must be 36 char long
        private static string SecretKey = "swxmkjiu543dsefw5678hvsgtolkbD32SXDF";

        public static string GenerateToken(string userid)
        {
            byte[] key = Convert.FromBase64String(SecretKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[] { new Claim(type: ClaimTypes.Name, value: userid) }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials=new SigningCredentials(securityKey,algorithm:SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);

        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if(jwtToken==null)
                {
                    return null;
                }
                byte[] key = Convert.FromBase64String(SecretKey);
                TokenValidationParameters parameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securitytoken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securitytoken);
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string ValidateToken(string token)
        {
            try
            {
                string userid = string.Empty;
                ClaimsPrincipal principal = GetPrincipal(token);
                if(principal==null)
                {
                    return null;
                }
                ClaimsIdentity identity = null;
                try
                {
                    identity = (ClaimsIdentity)principal.Identity;
                }
                catch (Exception ex)
                {
                    return null;
                }
                Claim useridClaim = identity.FindFirst(type: ClaimTypes.Name);
                userid = useridClaim.Value;
                return userid;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}