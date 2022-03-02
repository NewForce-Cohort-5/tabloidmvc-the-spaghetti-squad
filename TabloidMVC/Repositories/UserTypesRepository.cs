using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class UserTypesRepository : BaseRepository, IUserTypesRepository
    {
        public UserTypesRepository(IConfiguration config) : base(config) { }
        public List<UserType> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, name FROM UserType";
                    var reader = cmd.ExecuteReader();

                    var userType = new List<UserType>();

                    while (reader.Read())
                    {
                        userType.Add(new UserType()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                        });
                    }

                    reader.Close();

                    return userType;
                }
            }
        }
    }
}