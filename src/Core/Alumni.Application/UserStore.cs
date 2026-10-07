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
    }
}
