using System.Linq;
using RMS.Data.Entities;
using RMS.Data.Repository;
using RMS.Data.Security;

namespace RMS.Data.Services;

// EntityFramework Implementation of IUserService
public class UserServiceDb : IUserService
{
    private readonly DataContext db;

    public UserServiceDb()
    {
        db = new DataContext();
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // ==================== User Authentication/Registration Management ==================
    public User Authenticate(string email, string password)
    {
        // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
        var user = GetUserByEmail(email);

        // Verify the user exists and Hashed User password matches the password provided
        return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;
    }
    public User Register(string name, string email, string password, Role role)
    {
        // check that the user does not already exist (unique user name)
        var exists = GetUserByEmail(email);
        if (exists != null)
        {
            return null;
        }

        // Custom Hasher used to encrypt the password before storing in database
        var user = new User 
        {
            Name = name,
            Email = email,
            Password = Hasher.CalculateHash(password),
            Role = role   
        };

        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }

    public User GetUserByEmail(string email)
    {
        return db.Users.FirstOrDefault(u => u.Email == email);
    }

    public User GetUserById(int id)
    {
        return db.Users.FirstOrDefault(u => u.Id == id);
    }

    public List<User> GetAllUsers()
    {
        return db.Users.ToList();
    }

    public User UpdateUserRole(int id, Role role)
    {
        var user = GetUserById(id);
        if (user is null) { return null; }
        user.Role = role;
        db.SaveChanges();
        return user;
    }

    public User UpdateUser(int id, string name, string email)
    {
        var user = GetUserById(id);
        if (user is null) { return null; }
        // email uniqueness check — reject if another user already uses this email
        var existing = GetUserByEmail(email);
        if (existing != null && existing.Id != id) { return null; }
        user.Name = name;
        user.Email = email;
        db.SaveChanges();
        return user;
    }

    public bool DeleteUser(int id)
    {
        var user = GetUserById(id);
        if (user is null) { return false; }
        db.Users.Remove(user);
        db.SaveChanges();
        return true;
    }

}

