using Alumni.Domain.Entities;

namespace Alumni.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
