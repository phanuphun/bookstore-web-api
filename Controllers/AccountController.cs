using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SampleWebAPI.Models;
using SampleWebAPI.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace SampleWebAPI.Controllers;

[ApiController]
[Route("api/")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;
    public AccountController(IConfiguration config ,AppDbContext db)
    {
        _config = config;
        _db = db;
    }

    [HttpGet("GetAccount")]
    [Authorize]
    public ActionResult<Account> GetAccount()
    {
        var account = _db.Accounts.ToList();
        account.Reverse();
        return Ok(account);
    }

    [HttpPost("Register")]
    public ActionResult Register([FromForm] Account Data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var exitingUser = _db.Accounts.FirstOrDefault(x => x.Username == Data.Username);
        if (exitingUser != null)
        {
            return BadRequest("Username already exists.");
        }

        string hashPassword = HashPassword(Data.Password);

        var user = new Account
        {
            FirstName = Data.FirstName,
            LastName = Data.LastName,
            Username = Data.Username,
            Email = Data.Email,
            Phone = Data.Phone,
            Password = hashPassword,
            Address = Data.Address,
        };

        _db.Accounts.Add(user);
        _db.SaveChanges();
        return Ok("Register Successfully!");
    }

    [HttpPost("Login")]
    public ActionResult<Login> Login([FromForm] Login data)
    {
        if (data.Username == null || data.Password == null)
        {
            return BadRequest("Username or password incorect");
        }

        var user = _db.Accounts.FirstOrDefault(u => u.Username == data.Username);
        if (user == null)
        {
            return BadRequest("Username or password incorect");
        }

        bool validate = VerifyPassword(data.Password, user.Password!);
        if (!validate)
        {
            return BadRequest("Username or password incorect");
        }

        // Create JWT Token
        var tokenHandler = new JwtSecurityTokenHandler(); // class jwt for create token
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]); // access jwt->key in appsetting.json
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim("id", user.Id.ToString())
            }),

            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"])),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"], 
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var res = new
        {
            message = "Login Successfully",
            user,
            token = tokenString
        };

        return Ok(res);
    }


    [HttpDelete("deleteAccount/{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Account> DeleteAccountById(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound("Not Found Usere");
        }
        var user = _db.Accounts.Find(id);
        if (user == null)
        {
            return NotFound("Not Found Usere");
        }
        _db.Accounts.Remove(user);
        _db.SaveChanges();
        return Ok("Deleted Successfully");
    }

    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        var hashOfInput = HashPassword(enteredPassword);
        return hashOfInput == storedHash;
    }
}


