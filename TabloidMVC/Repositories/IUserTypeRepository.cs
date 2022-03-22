using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserTypeRepository
    {
        public List<UserType> GetAll();
    }
}