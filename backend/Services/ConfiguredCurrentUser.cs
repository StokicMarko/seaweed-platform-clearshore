using Entities;
using Microsoft.Extensions.Options;

namespace Services;

/// Temporary ICurrentUser: always the same staff member (what else can i do now kill me), taken from configuration.
public class ConfiguredCurrentUser(IOptions<CurrentUserOptions> options) : ICurrentUser
{
    public int Id => options.Value.Id;
    public UserRole Role => options.Value.Role;
}