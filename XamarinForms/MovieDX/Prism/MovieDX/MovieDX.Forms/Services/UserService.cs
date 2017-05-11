using MovieDX.Core.Helpers;
using MovieDX.Forms.DataAccess;
using MovieDX.Forms.Interfaces;
using MovieDX.Forms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MovieDX.Forms.Services
{
    public class UserService
    {
        private readonly IRepository<User> _userRepo;
        public UserService()
        {
            var connectionService = DependencyService.Get<ISQLite>();
            _userRepo = new Repository<User>(connectionService);
        }

        public bool IsUserAuthenticated()
        {
            var result = false;

            try
            {
                var user = _userRepo.GetAll();

                if (user?.Find(x => x.IsLoggedIn == true) != null)
                {
                    result = true;
                }
            }
            catch (System.Exception ex)
            {
                ErrorLog.LogError("ERROR: User Auth", ex);
            }

            return result;
        }

        public async Task<User> GetActiveUser()
        {
            var user = new User();
            try
            {
                var users = await _userRepo.GetAllAsync();

                user = users.FirstOrDefault(x => x.IsLoggedIn);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("ERROR: Getting active user", ex);
            }

            return user;
        }
    }
}
