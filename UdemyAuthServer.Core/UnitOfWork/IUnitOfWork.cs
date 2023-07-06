namespace UdemyAuthServer.Core.UnitOfWork;

public interface IUnitOfWork
{
    Task SaveChancesAsync();
    void SaveChances();
}
