using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace CRUD1.Services
{
    public class TokenService : ITokenService
    {
        // GenerateAccessToken() contains the logic to generate the access token. This is a familiar logic that we already have in the AuthController 
        //(from a previous article), which we are going to remove from there.
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                 //The first parameter is a simple string representing the name of the webserver that issues the token
                 issuer: "https://localhost:5001",
                 //The second parameter is a string value representing valid recipients
                 audience: "https://localhost:5001",
                //The third argument is a list of user roles, for example, the user can be an admin, manager, or author 
                claims: claims,
                 //The fourth argument is the DateTime object that represents the date and time after which the token expires
                 expires: DateTime.Now.AddMinutes(5),
                 signingCredentials: signinCredentials
             );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return  tokenString ;
        }

        //GenerateRefreshToken() contains the logic to generate the refresh token. 
        //We use the RandomNumberGenerator class to generate a cryptographic random number for this purpose.
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //GetPrincipalFromExpiredToken() is used to get the user principal from the expired access token. We make use of the ValidateToken() method of JwtSecurityTokenHandler class for this purpose. 
        //This method validates the token and returns the ClaimsPrincipal object.
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345")),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
    
}
