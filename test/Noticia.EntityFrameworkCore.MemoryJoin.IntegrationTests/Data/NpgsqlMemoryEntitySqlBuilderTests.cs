using System.Linq;
using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;

[Collection("NpgsqlDbFixture")]
public class NpgsqlMemoryEntitySqlBuilderTests
{
    #region Methods
    
    [Fact]
    public void Should_BuildMemoryEntitySqlQuery_When_ModelsPassed()
    {
        var models = new[]
        {
            new TestModel(){StringValue = "abc", IntValue = 23},
            new TestModel(){StringValue = "def", IntValue = 453},
            new TestModel(){StringValue = "ghi", IntValue = 476},
            new TestModel(){StringValue = "jkl", IntValue = 5},
            new TestModel(){StringValue = "mno", IntValue = 85}
        };

        var mappings = new MemoryEntityMapping<TestModel>();
        var memoryEntitySqlBuilder = new NpgsqlMemoryEntitySqlQueryBuilder<TestModel>(mappings, models.Select(model=>mappings.ToFunc(model)).ToList());
        var expected = "select * from (values (@memoryEntity_0,@memoryEntity_1,@memoryEntity_2),(@memoryEntity_3,@memoryEntity_4,@memoryEntity_5),(@memoryEntity_6,@memoryEntity_7,@memoryEntity_8),(@memoryEntity_9,@memoryEntity_10,@memoryEntity_11),(@memoryEntity_12,@memoryEntity_13,@memoryEntity_14)) as TestModel_entity(Id,StringValue,IntValue) ";
        
        Assert.Equal(expected, memoryEntitySqlBuilder.Build());
    }
    
    #endregion
}