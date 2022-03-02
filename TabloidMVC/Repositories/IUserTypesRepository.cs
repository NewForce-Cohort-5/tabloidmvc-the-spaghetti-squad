using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserTypesRepository
    {
        List<UserType> GetAll();
    }
}