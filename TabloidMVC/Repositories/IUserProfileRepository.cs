﻿using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile GetByEmail(string email);
        List <UserProfile> GetAllUsers();

        void Add(UserProfile user);

    }
}