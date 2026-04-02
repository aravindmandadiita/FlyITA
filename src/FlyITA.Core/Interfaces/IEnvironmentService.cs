namespace FlyITA.Core.Interfaces;

public interface IEnvironmentService
{
    string Role { get; }
    bool IsClientFacing { get; }
    string GetConnectionStringName();
}
