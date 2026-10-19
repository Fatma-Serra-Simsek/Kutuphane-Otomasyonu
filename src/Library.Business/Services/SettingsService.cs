using Library.Business.Results;
using Library.Domain.Repositories;

namespace Library.Business.Services;

public class SettingsService : ISettingsService
{
    private readonly IUnitOfWork _uow;

    public SettingsService(IUnitOfWork uow) => _uow = uow;

    public async Task<int> GetLoanPeriodDaysAsync(CancellationToken cancellationToken = default)
    {
        var value = await _uow.Settings.GetValueAsync(LoanService.LoanPeriodDaysKey, cancellationToken);
        return int.TryParse(value, out var days) && days > 0
            ? days
            : LoanService.DefaultLoanPeriodDays;
    }

    public async Task<ServiceResult> SetLoanPeriodDaysAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        if (days < 1)
            return ServiceResult.Fail("Ödünç süresi en az 1 gün olmalıdır.");

        await _uow.Settings.SetValueAsync(
            LoanService.LoanPeriodDaysKey,
            days.ToString(),
            cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
