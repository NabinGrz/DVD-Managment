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
        //            public int UserNumber { get; set; }
        //[Required(ErrorMessage = "Name is required")]
        //public string UserName { get; set; }
        //[Required(ErrorMessage = "User type is required")]
        //public string UserType { get; set; }
        //public string UserPassword { get; set; }

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
