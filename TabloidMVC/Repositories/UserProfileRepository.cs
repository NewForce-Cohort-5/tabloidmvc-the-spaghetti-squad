using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration config) : base(config) { }

        public UserProfile GetByEmail(string email)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT u.id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                              u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                              ut.[Name] AS UserTypeName, u.Deactivated
                         FROM UserProfile u
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE email = @email";
                    cmd.Parameters.AddWithValue("@email", email);

                    UserProfile userProfile = null;
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        userProfile = NewUserFromReader(reader);
                    }

                    reader.Close();

                    return userProfile;
                }
            }
        }

        public List <UserProfile> GetAllUsers()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT u.id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                              u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                              ut.[Name] AS UserTypeName, u.Deactivated
                         FROM UserProfile u
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                               ORDER BY u.DisplayName ASC;";

                    var reader = cmd.ExecuteReader();

                    var users = new List<UserProfile>();

                    while (reader.Read())
                    {
                        users.Add(NewUserFromReader(reader));
                    }

                    reader.Close();

                    return users;
                }
            }
        }
        public List<UserProfile> GetActiveUsers()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT u.id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                              u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                              ut.[Name] AS UserTypeName, u.Deactivated
                         FROM UserProfile u
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                                WHERE u.Deactivated = 0
                               ORDER BY u.DisplayName ASC;";

                    var reader = cmd.ExecuteReader();

                    var users = new List<UserProfile>();

                    while (reader.Read())
                    {
                        users.Add(NewUserFromReader(reader));
                    }

                    reader.Close();

                    return users;
                }
            }
        }
        public UserProfile GetUserById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT u.id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                              u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                              ut.[Name] AS UserTypeName, u.Deactivated
                         FROM UserProfile u
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id                
                            WHERE u.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    UserProfile user = null;

                    if (reader.Read())
                    {
                        user = NewUserFromReader(reader);
                    }

                    reader.Close();

                    return user;
                }
            }
        }

        public void Add(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       insert into UserProfile  (DisplayName, FirstName, LastName, Email, CreateDateTime, ImageLocation, UserTypeId, Deactivated)
                              OUTPUT INSERTED.ID                   
                    VALUES (@DisplayName, @FirstName, @LastName, @Email, @CreateDateTime, @ImageLocation, @UserTypeId, @Deactivated)";
                    cmd.Parameters.AddWithValue("@DisplayName", user.DisplayName);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", (user.LastName));
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ImageLocation", DbUtils.ValueOrDBNull(user.ImageLocation));
                    cmd.Parameters.AddWithValue("@UserTypeId", 2);
                    cmd.Parameters.AddWithValue("@Deactivated", false);


                    user.Id = (int)cmd.ExecuteScalar();
                }

            }
        }

        public void DeactivateUser(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE UserProfile
                            SET Deactivated = @Deactivated                           
                            WHERE id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@Deactivated", true);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Reusable SQLreader for UserProfile
        private UserProfile NewUserFromReader(SqlDataReader reader)
        {
            return new UserProfile()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                ImageLocation = DbUtils.GetNullableString(reader, "ImageLocation"),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Deactivated = reader.GetBoolean(reader.GetOrdinal("Deactivated")),
                UserType = new UserType()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                    Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                }
                 
                
            };
        }
    }
}
