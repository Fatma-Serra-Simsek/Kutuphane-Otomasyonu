using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public class SettingRepository : ISettingRepository
{
    private readonly LibraryDbContext _db;

    public SettingRepository(LibraryDbContext db) => _db = db;

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await _db.Settings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        return setting?.Value;
    }

    public async Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var setting = await _db.Settings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (setting is null)
        {
            _db.Settings.Add(new Setting { Key = key, Value = value });
        }
        else
        {
            setting.Value = value;
        }
    }
}
