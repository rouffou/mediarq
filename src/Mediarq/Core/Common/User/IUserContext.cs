namespace Mediarq.Core.Common.User;

public interface IUserContext
{
    string UserId { get; }
    string UserName { get; }
    IEnumerable<string> Roles { get; }
}
