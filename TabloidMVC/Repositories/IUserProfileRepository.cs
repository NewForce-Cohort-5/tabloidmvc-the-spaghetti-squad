using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile GetByEmail(string email);
        List <UserProfile> GetAllUsers();
        UserProfile GetUserById(int id);
        void Add(UserProfile user);
        void DeactivateUser(int id);
        void ReactivateUser(int id);
        void UpdateUserType(UserProfile userProfile);

    }
}