using Library.Domain.Enums;

namespace Library.Business.Services;

public static class CopyStatusTransitions
{
    private static readonly Dictionary<CopyStatus, HashSet<CopyStatus>> Allowed = new()
    {
        [CopyStatus.Available] = [CopyStatus.Loaned, CopyStatus.Lost, CopyStatus.Damaged],
        [CopyStatus.Loaned] = [CopyStatus.Available, CopyStatus.Lost],
        [CopyStatus.Damaged] = [CopyStatus.Available, CopyStatus.Lost],
        [CopyStatus.Lost] = [CopyStatus.Available]
    };

    public static bool CanTransition(CopyStatus from, CopyStatus to) =>
        from != to && Allowed.TryGetValue(from, out var next) && next.Contains(to);
}
