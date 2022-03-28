using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Data;

public class NpgsqlMemoryEntitySqlBuilderTests : IClassFixture<DbFixture>
{
    private readonly TestDbContext testDbContext;
    
    public NpgsqlMemoryEntitySqlBuilderTests(DbFixture dbFixture)
    {
        this.testDbContext = dbFixture.TestDbContext;
    }
    
    [Fact]
    public void Should()
    {
        var entities = new[]
        {
            new TestModel(){StringValue = "abc", IntValue = 23},
            new TestModel(){StringValue = "def", IntValue = 453},
            new TestModel(){StringValue = "ghi", IntValue = 476},
            new TestModel(){StringValue = "jkl", IntValue = 5},
            new TestModel(){StringValue = "mno", IntValue = 85}
        };

        var mappings = new MemoryEntityMapping<TestModel>();
        
        var connection = this.testDbContext.Database.GetDbConnection();
        using (var command = connection.CreateCommand())
        {
            var memoryEntitySqlBuilder = new NpgsqlMemoryEntitySqlBuilder<TestModel>(mappings, entities, command);

            var x = memoryEntitySqlBuilder.Build();

            var y = 5;
        }
    }
}