using RopeyDVDs.Data;
using RopeyDVDs.Models;
using System.Security.Claims;

namespace RopeyDVDs.Services
{
    public class UserServices
    {
        private readonly RopeyDVDsContext _context;
        public UserServices(RopeyDVDsContext context)
        {
            _context = context;
        }

        internal User GetUserById(int UserNumber)
        {
            var appUser = _context.Users.Find(UserNumber);
            return appUser;
        }

        internal bool TryValidateUser(string username, string password, out List<Claim> claims)
        {

        claims = new List<Claim>();
            var appUser = _context.Users
                .Where(a => a.UserName == username)
                .Where(a => a.UserPassword == password).FirstOrDefault();
            if (appUser is null)
            {
                return false;
            }
            else
            {
                claims.Add(new Claim("UserNumber", appUser.UserNumber.ToString()));
                claims.Add(new Claim("UserName", appUser.UserName));
                claims.Add(new Claim("UserType", appUser.UserType));
                claims.Add(new Claim("UserPassword", appUser.UserPassword));
            }
            return true;
        }
    }

}
