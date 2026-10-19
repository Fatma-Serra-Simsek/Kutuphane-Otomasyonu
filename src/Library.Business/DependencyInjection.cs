using FluentValidation;
using Library.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddLibraryBusiness(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<ICatalogLookupService, CatalogLookupService>();

        return services;
    }
}
