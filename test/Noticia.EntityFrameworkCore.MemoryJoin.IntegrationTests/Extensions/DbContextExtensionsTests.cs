using System.Linq;
using Noticia.EntityFrameworkCore.MemoryJoin.Extensions;
using Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Extensions;

[Collection("NpgsqlDbFixture")]
public class DbContextExtensionsTests : IClassFixture<DbFixture>
{
    #region Fields

    private readonly DbFixture dbFixture;

    #endregion

    #region Constructors

    public DbContextExtensionsTests(DbFixture dbFixture)
    {
        this.dbFixture = dbFixture;

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

        using (var dbContext = this.dbFixture.CreateContext())
        {
            dbContext.JoinableTableModels.AddRange(joinableTableModels);
            dbContext.SaveChanges();
        }
    }

    #endregion

    #region Methods

    [Fact]
    public void Should_JoinTablesCorrectly_When_ModelsUsed()
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

        using (var dbContext = this.dbFixture.CreateContext())
        {
            var memoryEntities = dbContext.AsMemoryEntities(models);

            var stringValues = dbContext.JoinableTableModels
                .Join(memoryEntities, outer => outer.StringValue, inner => inner.StringValue,
                    (outer, inner) => new { outer, inner })
                .OrderBy(arg => arg.inner.IntValue)
                .Select(arg => arg.outer.StringValue)
                .ToList();

            Assert.Equal(new[] { "stringValue3", "stringValue1", "stringValue2" }, stringValues);

            dbContext.ChangeTracker.Clear();
        }
    }
    
    [Fact]
    public void Should_ReturnEmptyCollection_When_NoModelsUsed()
    {
        var models = new TestModel[]
        {
        };

        using (var dbContext = this.dbFixture.CreateContext())
        {
            var memoryEntities = dbContext.AsMemoryEntities(models);

            var stringValues = dbContext.JoinableTableModels
                .Join(memoryEntities, outer => outer.StringValue, inner => inner.StringValue,
                    (outer, inner) => new { outer, inner })
                .OrderBy(arg => arg.inner.IntValue)
                .Select(arg => arg.outer.StringValue)
                .ToList();

            Assert.Empty(stringValues);
                
            dbContext.ChangeTracker.Clear();
        }
    }

    #endregion
}