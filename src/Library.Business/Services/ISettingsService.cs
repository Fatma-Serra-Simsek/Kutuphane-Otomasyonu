using Library.Business.Results;

namespace Library.Business.Services;

public interface ISettingsService
{
    Task<int> GetLoanPeriodDaysAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> SetLoanPeriodDaysAsync(int days, CancellationToken cancellationToken = default);
}
