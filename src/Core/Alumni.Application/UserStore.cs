using Alumni.Domain.Entities;

namespace Alumni.Application
{
    public static class UserStore
    {
        public static readonly object UsersLock = new();
        public static readonly List<User> Users = new()
        {
            new User { Id = 1, FirstName = "Ahmet", LastName = "Yılmaz", Email = "ahmet.yilmaz@example.com", Role = "Admin", CreatedAt = DateTime.UtcNow },
            new User { Id = 2, FirstName = "Ayşe", LastName = "Demir", Email = "ayse.demir@example.com", Role = "Alumni", CreatedAt = DateTime.UtcNow }
        };

        public static List<User> GetAll()
        {
            lock (UsersLock)
            {
                return Users.ToList();
            }
        }

        public static User? GetById(int id)
        {
            lock (UsersLock)
            {
                return Users.FirstOrDefault(u => u.Id == id);
            }
        }

        public static User Add(User newUser)
        {
            lock (UsersLock)
            {
                newUser.Id = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1;
                newUser.CreatedAt = DateTime.UtcNow;
                Users.Add(newUser);
                return newUser;
            }
        }

        public static bool Update(int id, string? firstName, string? lastName, string? email, string? role)
        {
            lock (UsersLock)
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return false;

                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.Role = role;
                return true;
            }
        }

        public static bool Patch(int id, string? firstName, string? lastName, string? email, string? role)
        {
            lock (UsersLock)
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return false;

                if (!string.IsNullOrWhiteSpace(firstName)) user.FirstName = firstName;
                if (!string.IsNullOrWhiteSpace(lastName)) user.LastName = lastName;
                if (!string.IsNullOrWhiteSpace(email)) user.Email = email;
                if (!string.IsNullOrWhiteSpace(role)) user.Role = role;
                return true;
            }
        }

        public static bool Delete(int id)
        {
            lock (UsersLock)
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return false;

                Users.Remove(user);
                return true;
            }
        }
    }
}

