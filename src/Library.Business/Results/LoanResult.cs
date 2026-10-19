using Library.Domain.Entities;

namespace Library.Business.Results;

public class LoanResult
{
    public LoanResultCode Code { get; init; }
    public Loan? Loan { get; init; }
    public string? Message { get; init; }

    public bool IsSuccess => Code == LoanResultCode.Success;

    public static LoanResult Ok(Loan loan) => new()
    {
        Code = LoanResultCode.Success,
        Loan = loan
    };

    public static LoanResult Fail(LoanResultCode code, string? message = null) => new()
    {
        Code = code,
        Message = message
    };
}
