using System.Reflection;
using Xunit;

namespace XeoTechErp.Tests;

public sealed class ArchitectureDependencyTests
{
    private static readonly Assembly Domain = typeof(XeoTechErp.Domain.Entities.Product).Assembly;
    private static readonly Assembly Application = typeof(XeoTechErp.Application.Abstractions.Persistence.IUnitOfWork).Assembly;
    private static readonly Assembly Infrastructure = typeof(XeoTechErp.Infrastructure.Persistence.XeoTechDbContext).Assembly;
    private static readonly Assembly Api = typeof(XeoTechErp.Api.Program).Assembly;

    [Fact]
    public void Domain_must_not_reference_infrastructure_application_api_or_ef_core()
    {
        var references = ReferencedAssemblyNames(Domain);

        Assert.DoesNotContain(references, x => x is "XeoTechErp.Application" or "XeoTechErp.Infrastructure" or "XeoTechErp.Api");
        Assert.DoesNotContain(references, x => x.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal));
    }

    [Fact]
    public void Application_must_not_reference_infrastructure_or_api()
    {
        var references = ReferencedAssemblyNames(Application);

        Assert.DoesNotContain(references, x => x is "XeoTechErp.Infrastructure" or "XeoTechErp.Api");
    }

    [Fact]
    public void Infrastructure_must_not_reference_api()
    {
        var references = ReferencedAssemblyNames(Infrastructure);

        Assert.DoesNotContain(references, x => x is "XeoTechErp.Api");
    }

    [Fact]
    public void Api_may_depend_on_infrastructure_and_application_but_domain_remains_independent()
    {
        var references = ReferencedAssemblyNames(Api);

        Assert.Contains("XeoTechErp.Application", references);
        Assert.Contains("XeoTechErp.Infrastructure", references);
        Assert.DoesNotContain("XeoTechErp.Tests", references);
    }

    private static HashSet<string> ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);
}
