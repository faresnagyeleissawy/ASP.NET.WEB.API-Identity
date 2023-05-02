using EmployeeIdentity.APIs.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.Xml;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using EmployeeIdentity.APIs.Data.Models;

namespace EmployeeIdentity.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        public readonly IConfiguration _configration;
        private readonly UserManager<Employee> _userManager;

        public UserController(IConfiguration configuration, UserManager<Employee> userManager)
        {
            _configration = configuration;
            _userManager = userManager;
        }

        #region static-login

        [HttpPost]
        [Route("Static-login")]
        public ActionResult<TokenDto> StaticLogin (LoginDto credintials) 
        {
            if (credintials.Password != "password" || credintials.UserName != "username")
            {
                return BadRequest();
            }
            var calimsList = new List<Claim>
            {
                new  Claim("Nationlaity", "Egyptian"),
                new  Claim(ClaimTypes.NameIdentifier,"id"),
                new  Claim(ClaimTypes.Role,"Manager"),
            };
            string? secretKey = _configration.GetValue<string>("SecretKey")!;
            var keyByteArray = Encoding.ASCII.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(keyByteArray);
            var singingCreadinatials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);
            var expiray = DateTime.Now.AddMinutes(30);
            var jwt = new JwtSecurityToken(

                claims: calimsList,
                signingCredentials: singingCreadinatials,
                expires: expiray
                );

            var tokenHandeler= new JwtSecurityTokenHandler();
            
            string tokenString = tokenHandeler.WriteToken(jwt);
            return new TokenDto
            {
                Token = tokenString,
                Expiry = expiray
            };
                
                
            
        }
        #endregion
        #region register
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult>Register(RegisterDto registerDto)
        {
            var newEmp = new Employee
            {
                UserName = registerDto.UserName,
                Department = registerDto.Department,
                Email = registerDto.Email,
            };
            var result = await _userManager.CreateAsync(newEmp, registerDto.Password);
            if (result is null)
            {
                return BadRequest();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,registerDto.Password),
                new Claim(ClaimTypes.Role,"Employee"),
            };
            await _userManager.AddClaimsAsync(newEmp, claims);
            
            return NoContent();
        }



        #endregion
        #region login

        [HttpPost]
        [Route("login")]
        public async Task <ActionResult<TokenDto>> Login(LoginDto credintials)
        {
            Employee? user = await _userManager.FindByNameAsync(credintials.UserName);
            if (user == null) { return BadRequest(); };
            bool isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked) { return BadRequest(); };
            bool isAthunticated = await _userManager.CheckPasswordAsync(user, credintials.Password);
            if (!isAthunticated) 
            {
                await _userManager.AccessFailedAsync(user);
                return BadRequest();
            };
            var claimsList = await _userManager.GetClaimsAsync(user);
            string? secretKey = _configration.GetValue<string>("SecretKey")!;
            var keyByteArray = Encoding.ASCII.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(keyByteArray);
            var singingCreadinatials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expiray = DateTime.Now.AddMinutes(30);
            var jwt = new JwtSecurityToken(

                claims: claimsList,
                signingCredentials: singingCreadinatials,
                expires: expiray
                );

            var tokenHandeler = new JwtSecurityTokenHandler();

            string tokenString = tokenHandeler.WriteToken(jwt);
            return new TokenDto
            {
                Token = tokenString,
                Expiry = expiray
            };
        }



        #endregion
    }
}
