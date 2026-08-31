using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Exceptions;

namespace XeoTechErp.Tests;

public sealed class DomainRulesTests
{
    [Fact]
    public void Product_cannot_have_negative_stock()
    {
        Assert.Throws<DomainRuleException>(() =>
            new Product("SKU-1", "Product", 100m, 50m, -1, 0));
    }

    [Fact]
    public void Product_cannot_reduce_stock_below_zero()
    {
        var product = new Product("SKU-1", "Product", 100m, 50m, 2, 1);

        Assert.Throws<DomainRuleException>(() => product.DecreaseStock(3));
    }

    [Fact]
    public void Adding_order_item_updates_stock_and_subtotal()
    {
        var product = new Product("SKU-1", "Product", 100m, 50m, 10, 1);
        var order = new Order(1);

        order.AddItem(product, 3);

        Assert.Equal(7, product.Stock);
        Assert.Equal(300m, order.Subtotal);
        Assert.Equal(300m, order.Total);
    }

    [Fact]
    public void Order_discount_cannot_exceed_one_hundred_percent()
    {
        var order = new Order(1);

        Assert.Throws<DomainRuleException>(() => order.ApplyDiscount(101m));
    }

    [Fact]
    public void Cancelled_order_cannot_be_cancelled_twice()
    {
        var order = new Order(1);
        order.Cancel();

        Assert.Throws<DomainRuleException>(order.Cancel);
    }
}
