namespace Dddify.Admin.Infrastructure.Services;

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class GuidGenerator : IGuidGenerator
{
    public Guid Create() => Guid.CreateVersion7();
}