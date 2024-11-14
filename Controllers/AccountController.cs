using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SampleWebAPI.Models;
using SampleWebAPI.Services;

namespace SampleWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{

    private readonly AppDbContext _db;
    public AccountController(AppDbContext db)
    {
        _db = db;
    }

    [Route("GetAccount")]
    [HttpGet]
    public ActionResult<Account> GetAccount()
    {
        var account = _db.Accounts.ToList();
        account.Reverse();
        return Ok(account);
    }

    [Route("Register")]
    [HttpPost]

    public ActionResult Register([FromBody] Account Data)
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

        return Ok("Delete Successfully");
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


