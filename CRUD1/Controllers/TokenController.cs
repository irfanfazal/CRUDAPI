using CRUD1.Model;
using CRUD1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        private readonly usersContext _context;
        private readonly ITokenService _tokenService;


        public TokenController(usersContext context, ITokenService tokenService)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _tokenService = tokenService ?? throw new ArgumentException(nameof(tokenService));
        }

        /*
         We decorate our Login action with the HttpPost attribute. Inside the login method, 
        we create the SymmetricSecretKey with the secret key value superSecretKey@345. 
        Then, we create the SigningCredentials object and as arguments,
        we provide a secret key and the name of the algorithm that we are going to use to encode the token.
         */
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
            {
                return BadRequest("Invalid client reques");
            }

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default


            var user = _context.loginmodel.SingleOrDefault(u => u.UserName == username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client req");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            
            user.RefreshToken = newRefreshToken;
            _context.SaveChanges();

            return Ok(new AuthenticatedResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });

        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var user = _context.loginmodel.SingleOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
                return BadRequest();

            user.RefreshToken = null;
            _context.SaveChanges();

            return NoContent();
        }
    }
}
/*
 Here, we implement a refresh endpoint, which gets the user information from the expired access token and validates the refresh token against the user. 
Once the validation is successful,we generate a new access token and refresh token and the new refresh token is saved against the user in DB.
We also implement a revoke endpoint that invalidates the refresh token.
 */
