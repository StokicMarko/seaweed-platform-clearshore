using Entities;

namespace Services;

/// User making the current request.
/// (Login does not exist yet, so for now the id comes from configuration (ConfiguredCurrentUser)._
/// When login is built --> add another implementation that reads the logged-in user :,
public interface ICurrentUser
{
    int Id { get; }
    UserRole Role { get; }
    
}