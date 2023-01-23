using CRUD1.Model;
using CRUD1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CRUD1.Controllers

{
    [Route("api/[controller]")]
    [ApiController]

    /*
     First, we inject the UserContext and TokenService. Then, we validate the user credentials against the database. Once validation is successful, 
      we need to generate a refresh token in addition to the access token and save it along with the expiry date in the database:
     */
    public class AuthController : ControllerBase
    {
        private readonly usersContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(usersContext context, ITokenService tokenService)
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
        [HttpPost("login")]
        public IActionResult Login([FromBody] loginmodel userobj)
        {
            if (userobj is null)
            {
                return BadRequest("Invalid client request");
            }

            var user = _context.loginmodel.FirstOrDefault(x => x.UserName == userobj.UserName && x.Password==userobj.Password);
            if (user is null)
                return Unauthorized();


             var claims = new List<Claim>()
              {
                 new Claim(ClaimTypes.Name,userobj.UserName),
                 new Claim(ClaimTypes.Role,"Admin"),
                  new Claim(ClaimTypes.Role,"User"),
                };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);

            _context.SaveChanges();
            return Ok(new AuthenticatedResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });



        }
        [HttpPost("register")]
        public async Task<ActionResult<users>> Register(loginmodel userobj)
        {
            _context.loginmodel.Add(userobj);
            await _context.SaveChangesAsync();

            return Ok(new
            { Message = "User registered" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<loginmodel>>> Getaccount()
        {
            return await _context.loginmodel.ToListAsync();
        }
        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<loginmodel>> Getaccount(int id)
        {
            var login = await _context.loginmodel.FindAsync(id);

            if (login == null)
            {
                return NotFound();
            }

            return login;
        }
    }
}
/*
        Then, we create a string representation of JWT by calling the WriteToken method on JwtSecurityTokenHandler. 
       Finally, we return JWT in a response. 
       As a response, we create the AuthenticatedResponse object that contains only the Token property.
       var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);*/