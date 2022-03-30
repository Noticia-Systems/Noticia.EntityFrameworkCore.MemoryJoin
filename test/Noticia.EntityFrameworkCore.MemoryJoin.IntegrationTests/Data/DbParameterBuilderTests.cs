using System.Linq;
using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;

public class DbParameterBuilderTests :IClassFixture<NpgsqlDbFixture>
{
    #region Fields

    private readonly TestDbContext testDbContext;

    #endregion
    
    #region Constructors

    public DbParameterBuilderTests(NpgsqlDbFixture dbFixture)
    {
        this.testDbContext = dbFixture.TestDbContext;
    }
    
    #endregion

    #region Methods

    [Fact]
    public void Should()
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
            new TestModel(){StringValue = "abc", IntValue = 23},
            new TestModel(){StringValue = "def", IntValue = 453},
            new TestModel(){StringValue = "ghi", IntValue = 476},
            new TestModel(){StringValue = "jkl", IntValue = 5},
            new TestModel(){StringValue = "mno", IntValue = 85}
        };

        using (var dbCommand = testDbContext.Database.GetDbConnection().CreateCommand())
        {
              
            var mappings = new MemoryEntityMapping<TestModel>();
            var dbParameterBuilder =
                new DbParameterBuilder<TestModel>(mappings, models.Select(model => mappings.ToFunc(model)).ToList(), dbCommand);

            var dbParameters = dbParameterBuilder.Build();
            var i = 0;
            
            foreach (var dbParameter in dbParameters)
            {
                Assert.Equal(expected[i], dbParameter.Value);
                
                i++;
            }
        }
    }
    
    #endregion
}