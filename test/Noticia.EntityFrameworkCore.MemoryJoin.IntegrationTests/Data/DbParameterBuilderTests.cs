using System.Linq;
using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;

[Collection("NpgsqlDbFixture")]
public class DbParameterBuilderTests : IClassFixture<DbFixture>
{
    #region Fields

    private readonly DbFixture dbFixture;

    #endregion

    #region Constructors

    public DbParameterBuilderTests(DbFixture dbFixture)
    {
        this.dbFixture = dbFixture;
    }

    #endregion

    #region Methods

    [Fact]
    public void Should_GenerateParameters_When_ValidModelsPassed()
    {
        var expected = new object[]
        {
            1, "abc", 23,
            2, "def", 453,
            3, "ghi", 476,
            4, "jkl", 5,
            5, "mno", 85
        };

        var models = new[]
        {
            new TestModel() { StringValue = "abc", IntValue = 23 },
            new TestModel() { StringValue = "def", IntValue = 453 },
            new TestModel() { StringValue = "ghi", IntValue = 476 },
            new TestModel() { StringValue = "jkl", IntValue = 5 },
            new TestModel() { StringValue = "mno", IntValue = 85 }
        };

        using (var dbContext = dbFixture.CreateContext())
        {
            using (var dbCommand = dbContext.Database.GetDbConnection().CreateCommand())
            {
                var mappings = new MemoryEntityMapping<TestModel>();
                var dbParameterBuilder =
                    new DbParameterBuilder<TestModel>(mappings, models.Select(model => mappings.ToFunc(model)).ToList(),
                        dbCommand);

                var dbParameters = dbParameterBuilder.Build();
                var i = 0;

                foreach (var dbParameter in dbParameters)
                {
                    Assert.Equal(expected[i], dbParameter.Value);

                    i++;
                }
            }

            dbContext.ChangeTracker.Clear();
        }
    }

    #endregion
}