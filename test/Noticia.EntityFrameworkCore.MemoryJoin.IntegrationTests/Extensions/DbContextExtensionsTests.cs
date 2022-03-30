using System.Linq;
using Noticia.EntityFrameworkCore.MemoryJoin.Extensions;
using Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Extensions;

public class DbContextExtensionsTests : IClassFixture<NpgsqlDbFixture>
{
    #region Fields

    private readonly TestDbContext testDbContext;

    #endregion

    #region Constructors

    public DbContextExtensionsTests(NpgsqlDbFixture dbFixture)
    {
        this.testDbContext = dbFixture.TestDbContext;

        var joinableTableModels = new[]
        {
            new JoinableTableModel()
            {
                Test = "test1",
                StringValue = "stringValue1"
            },
            new JoinableTableModel()
            {
                Test = "test2",
                StringValue = "stringValue2"
            },
            new JoinableTableModel()
            {
                Test = "test3",
                StringValue = "stringValue3"
            }
        };

        this.testDbContext.JoinableTableModels.AddRange(joinableTableModels);
        this.testDbContext.SaveChanges();
    }

    #endregion

    #region Methods

    [Fact]
    public void Should()
    {
        var models = new[]
        {
            new TestModel()
            {
                StringValue = "stringValue1",
                IntValue = 2
            },
            new TestModel()
            {
                StringValue = "stringValue2",
                IntValue = 3
            },
            new TestModel()
            {
                StringValue = "stringValue3",
                IntValue = 1
            }
        };

        var memoryEntities = testDbContext.AsMemoryEntities(models);

        var stringValues = this.testDbContext.JoinableTableModels
            .Join(memoryEntities, outer => outer.StringValue, inner => inner.StringValue,
                (outer, inner) => new { outer, inner })
            .OrderBy(arg => arg.inner.IntValue)
            .Select(arg => arg.outer.StringValue)
            .ToList();

        //Assert.Equal("stringValue3", stringValues[0]);

        Assert.Equal(models.Select(model => model.StringValue), stringValues);
    }

    #endregion
}