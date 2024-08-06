using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeblogAspNet.Data;
using WeblogAspNet.Models;

public class AuthService
{
    private readonly ApplicationDBContext _context;

    public AuthService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<AppUser> AuthenticateUser(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null;
        }
        return user;
    }

    public async Task<bool> RegisterUser(RegisterModel model)
    {
        var user = new AppUser
        {
            Username = model.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            CreatedAt = DateTime.Now,
            IsAdmin = false,
            IsClose = false,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }
}