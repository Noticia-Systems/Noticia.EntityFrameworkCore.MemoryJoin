using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Reflection;

public class MemoryEntityManagerTests
{
    #region Methods

    [Fact]
    public void Should_CreateMemoryEntityTypeAndReturnSameTypeEachTime_When_Called()
    {
        var first = MemoryEntityManager.GetMemoryEntityType<TestModel>();
        var second = MemoryEntityManager.GetMemoryEntityType<TestModel>();

        Assert.Same(first, second);
    }
    
    #endregion
}