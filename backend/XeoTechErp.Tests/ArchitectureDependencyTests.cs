using System.Reflection;
using Xunit;

namespace XeoTechErp.Tests;

public sealed class ArchitectureDependencyTests
{
    [Fact]
    public void Domain_must_not_reference_entity_framework()
    {
        var references = typeof(XeoTechErp.Domain.Entities.Product).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(references, x => x.Name!.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal));
    }
}
