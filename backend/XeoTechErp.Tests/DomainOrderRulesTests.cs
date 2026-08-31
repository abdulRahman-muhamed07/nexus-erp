namespace XeoTechErp.Tests;

public sealed class DomainOrderRulesTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void Quantity_must_be_positive(int quantity, bool expected)
    {
        Assert.Equal(expected, quantity > 0);
    }
}
