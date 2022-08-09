using Microservice.Model;

namespace Microserivce.Interface
{
    public interface IUserService
    {
        User FindUser(int id);

        IEnumerable<User> UserAll();

    }
}