using Library.Domain.Entities;

namespace Library.Business.Results;

public class ReturnResult
{
    public LoanResultCode Code { get; init; }
    public Loan? Loan { get; init; }
    public int DaysOverdue { get; init; }
    public string? Message { get; init; }

    public bool IsSuccess => Code == LoanResultCode.Success;

    public static ReturnResult Ok(Loan loan, int daysOverdue) => new()
    {
        Code = LoanResultCode.Success,
        Loan = loan,
        DaysOverdue = daysOverdue
    };

    public static ReturnResult Fail(LoanResultCode code, string? message = null) => new()
    {
        Code = code,
        Message = message
    };
}
