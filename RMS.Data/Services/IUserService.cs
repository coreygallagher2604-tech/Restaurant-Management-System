using RMS.Data.Entities;
	
namespace RMS.Data.Services;

// This interface describes the operations that a UserService class should implement
public interface IUserService
{
    void Initialise();
        
    // ------------- User Management -------------------
    User Authenticate(string email, string password);
    User Register(string name, string email, string password, Role role);
    User GetUserByEmail(string email);

}
    
