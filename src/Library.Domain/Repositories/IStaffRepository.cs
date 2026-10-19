using Library.Domain.Entities;

namespace Library.Domain.Repositories;

public interface IStaffRepository
{
    Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Staff staff);
}
