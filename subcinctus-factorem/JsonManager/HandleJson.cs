using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace subcinctus_factorem.JsonManager
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class HandleJson
    {
        private string _jsonFilePath = "employee.json";
        public List<User> Users { get; private set; }

        public HandleJson()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(_jsonFilePath))
            {
                string json = File.ReadAllText(_jsonFilePath);
                Users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            else
            {
                Users = new List<User>();
            }
        }

        public void AddUser(User newUser)
        {
            Users.Add(newUser);
            SaveUsers();
        }

        public void DeleteUser(int userId)
        {
            var userToRemove = Users.FirstOrDefault(u => u.Id == userId);
            if (userToRemove != null)
            {
                Users.Remove(userToRemove);
                SaveUsers();
            }
        }

        public void UpdateUser(User updatedUser)
        {
            var existingUser = Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser != null)
            {
                existingUser.Name = updatedUser.Name;
                existingUser.Email = updatedUser.Email;
                SaveUsers();
            }
        }

        private void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(Users, Formatting.Indented);
            File.WriteAllText(_jsonFilePath, json);
        }
    }
}