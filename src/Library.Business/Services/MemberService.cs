using FluentValidation;
using Library.Business.Dtos;
using Library.Business.Results;
using Library.Domain.Common;
using Library.Domain.Entities;
using Library.Domain.Repositories;

namespace Library.Business.Services;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<MemberCreateDto> _createValidator;
    private readonly IValidator<MemberUpdateDto> _updateValidator;

    public MemberService(
        IUnitOfWork uow,
        IValidator<MemberCreateDto> createValidator,
        IValidator<MemberUpdateDto> updateValidator)
    {
        _uow = uow;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _uow.Members.GetByIdAsync(id, cancellationToken);

    public async Task<ServiceResult<Member>> CreateAsync(
        MemberCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return ServiceResult<Member>.Fail(validation.Errors[0].ErrorMessage);

        var member = new Member
        {
            MemberNumber = dto.MemberNumber.Trim(),
            FullName = dto.FullName.Trim(),
            Email = dto.Email.Trim(),
            Phone = dto.Phone?.Trim() ?? string.Empty,
            RegisteredAt = DateTime.Today,
            IsActive = true
        };

        _uow.Members.Add(member);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult<Member>.Ok(member);
    }

    public async Task<ServiceResult<Member>> UpdateAsync(
        MemberUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return ServiceResult<Member>.Fail(validation.Errors[0].ErrorMessage);

        var member = await _uow.Members.GetByIdAsync(dto.Id, cancellationToken);
        if (member is null)
            return ServiceResult<Member>.Fail("Üye bulunamadı.");

        if (member.IsActive && !dto.IsActive)
        {
            var activeLoans = await _uow.Loans.GetByMemberAsync(dto.Id, cancellationToken);
            if (activeLoans.Any(l => l.ReturnDate == null))
                return ServiceResult<Member>.Fail("Aktif ödüncü olan üye pasife alınamaz.");
        }

        member.MemberNumber = dto.MemberNumber.Trim();
        member.FullName = dto.FullName.Trim();
        member.Email = dto.Email.Trim();
        member.Phone = dto.Phone?.Trim() ?? string.Empty;
        member.IsActive = dto.IsActive;

        _uow.Members.Update(member);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult<Member>.Ok(member);
    }

    public async Task<ServiceResult> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var member = await _uow.Members.GetByIdAsync(id, cancellationToken);
        if (member is null)
            return ServiceResult.Fail("Üye bulunamadı.");

        var loans = await _uow.Loans.GetByMemberAsync(id, cancellationToken);
        if (loans.Any(l => l.ReturnDate == null))
            return ServiceResult.Fail("Aktif ödüncü olan üye pasife alınamaz.");

        member.IsActive = false;
        _uow.Members.Update(member);
        await _uow.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }

    public Task<PagedResult<Member>> SearchAsync(
        SearchQuery query,
        CancellationToken cancellationToken = default) =>
        _uow.Members.SearchAsync(query, cancellationToken);

    public Task<IReadOnlyList<Loan>> GetLoansAsync(
        int memberId,
        CancellationToken cancellationToken = default) =>
        _uow.Loans.GetByMemberAsync(memberId, cancellationToken);
}
