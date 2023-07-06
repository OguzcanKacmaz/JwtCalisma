using UdemyAuthServer.Core.UnitOfWork;

namespace UdemyAuthServer.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    public void SaveChances()
    {
        _context.SaveChanges();
    }

    public async Task SaveChancesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
