using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class UserRegisterViewModel
    {
        public UserProfile UserProfile { get; set; }
        public List<UserType> UserTypes { get; set; }
    }
}
