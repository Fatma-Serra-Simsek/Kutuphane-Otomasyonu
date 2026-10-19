using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly LibraryDbContext _db;

    public StaffRepository(LibraryDbContext db) => _db = db;

    public Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Staff.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public void Add(Staff staff) => _db.Staff.Add(staff);
}
