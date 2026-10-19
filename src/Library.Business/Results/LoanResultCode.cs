namespace Library.Business.Results;

public enum LoanResultCode
{
    Success = 0,
    MemberNotFound = 1,
    MemberInactive = 2,
    CopyNotFound = 3,
    CopyNotAvailable = 4,
    NoActiveLoan = 5
}
