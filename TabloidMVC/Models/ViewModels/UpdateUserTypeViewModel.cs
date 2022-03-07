using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class UpdateUserTypeViewModel
    {
       public UserProfile UserProfile { get; set; }
        public List <UserType> UserTypes { get; set; }
    }
}
