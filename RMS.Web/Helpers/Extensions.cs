using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RMS.Data.Entities;

namespace RMS.Web;

public static class Extensions
{
   
    // -------------------------- VIEW Authorisation Helper -------------------------//
    // ClaimsPrincipal - HasOneOfRoles extension method to check if a user has any of the roles in a comma separated string
    public static bool HasOneOfRoles(this ClaimsPrincipal claims, string rolesString)
    {
        // split string into an array of roles
        var roles = rolesString.Split(",");

        // linq query to check that ClaimsPrincipal has one of these roles
        return roles.FirstOrDefault(role => claims.IsInRole(role)) != null;
    }

    // --------------------------- AUTHENTICATION Helper ----------------------------//
    // IServiceCollection extension method adding cookie authentication 
    public static void AddCookieAuthentication(this IServiceCollection services, 
                                                    string notAuthorised = "/User/ErrorNotAuthorised", 
                                                    string notAuthenticated= "/User/ErrorNotAuthenticated")
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.AccessDeniedPath = notAuthorised;
                    options.LoginPath = notAuthenticated;
                });
    }

    // --------------------------- AUTHENTICATION Helper ----------------------------//
    // ClaimsPrincipal extension method to extract user id (sid) from claims
    public static int GetSignedInUserId(this ClaimsPrincipal user)
    {
        if (user != null && user.Identity != null && user.Identity.IsAuthenticated) {
            // id stored as a string in the Sid claim - convert to an int and return
            Claim sid = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid);
            if (sid == null) {
                throw new KeyNotFoundException("Sid Claim is not found in the identity");
            } 
            try {
                return Int32.Parse(sid.Value);  
            } catch (FormatException) {
                throw new KeyNotFoundException("Sid Claim value is invalid - not an integer");  
            }
        }
        return 0;
    }

    // ============================ IFormFile Helper ==============================//
    public static string ToBase64(this IFormFile file)
    {
        // check a file was uploaded
        if (file is null || file.Length <= 0) return null;

        string base64Encoding = string.Empty;
        using (var memoryStream = new MemoryStream())
        {
            file.CopyTo(memoryStream);
            base64Encoding = Convert.ToBase64String(memoryStream.ToArray());
        }
        return $"data:{file.ContentType};base64,{base64Encoding}";
    }
    
}

