using System.Text.RegularExpressions;
using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;
using PlanningPoker.Shared.ExtensionMethods;

namespace PlanningPoker.Domain.DomainModels;

public class Session : DomainModel<Guid>
{

    public string Code { get; }
    public string Password { get; private set; }
    public DateTimeOffset CreatedOn { get; }
    public DateTimeOffset RemovalOn { get; private set; }

    public void SetPassword(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            value = value.Hash(Code);
        }

        if (!Equals(Password, value))
        {
            Password = value;
        }
    }


    public Session(Guid id, 
        string code, 
        string password, 
        DateTimeOffset created,
        DateTimeOffset scheduled,
        DateTimeOffset removal) : base(id)
    {
        Code = code;
        Password = password;
        CreatedOn= created;
        RemovalOn = removal;
    }

    public Session() : base(Guid.NewGuid(), TrackingState.New)
    {
        Code = Randomizer.GenerateSessionCode();
        CreatedOn = DateTimeOffset.UtcNow;
        RemovalOn = DateTimeOffset.UtcNow.AddDays(7);
    }

    public static Session Create()
    {
        return new Session();
    }
}