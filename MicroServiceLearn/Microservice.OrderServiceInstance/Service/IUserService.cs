using Autofac.Extras.DynamicProxy;
using Microservice.Framework.PollyExtend;

namespace Microservice.OrderServiceInstance.Service
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效
    public interface IUserService
    {
        [PollyPolicyConfig(FallBackMethod = "UserServiceFallback",
            IsEnableCircuitBreaker = true,
            ExceptionsAllowedBeforeBreaking = 3,
            MillisecondsOfBreak = 1000 * 5
            )]
        User AOPGetById(int id);

        Task<User> GetById(int id);
    }

    public record User(int Id, string Name, string Account, string Password);
}
