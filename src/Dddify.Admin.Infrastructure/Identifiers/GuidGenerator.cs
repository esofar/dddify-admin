namespace Dddify.Admin.Infrastructure.Identifiers;

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class GuidGenerator : IGuidGenerator
{
    public Guid Create() => Guid.CreateVersion7();
}