using Xunit;

namespace XeoTechErp.Api.Tests;

public sealed class DomainValidationTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Positive_quantity_is_valid(int quantity)
    {
        Assert.True(quantity > 0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Non_positive_quantity_is_invalid(int quantity)
    {
        Assert.False(quantity > 0);
    }
}
