using System;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Extensions;

public class ModelBuilderExtensionsTests: IClassFixture<DbFixture>
{
    #region Fields

    private readonly TestDbContext testDbContext;

    #endregion
    
    #region Constructors

    public ModelBuilderExtensionsTests(DbFixture dbFixture)
    {
        this.testDbContext = dbFixture.TestDbContext;
    }
    
    #endregion
    
    #region Methods

    [Fact]
    public void Should_CreateEntityInDatabaseAndLoadValuesCorrectly_When_MemoryEntityUsed()
    {
        var entity = Activator.CreateInstance(MemoryEntityManager.GetMemoryEntityType<TestModel>()) ?? throw new InvalidOperationException("Could not create memory entity.");

        var idProperty = entity.GetType().GetProperty("Id") ?? throw new InvalidOperationException("Could not find Id property.");
        var stringValueProperty = entity.GetType().GetProperty("StringValue") ?? throw new InvalidOperationException("Could not find StringValue property.");
        var intValueProperty = entity.GetType().GetProperty("IntValue") ?? throw new InvalidOperationException("Could not find IntValue property.");

        idProperty.SetValue(entity, 42);
        stringValueProperty.SetValue(entity, "Test");
        intValueProperty.SetValue(entity, 12);

        this.testDbContext.Add(entity);
        this.testDbContext.SaveChanges();

        var loadedEntity = this.testDbContext.Find(entity.GetType(), 42);
 
        Assert.Equal(42, idProperty.GetValue(loadedEntity));
        Assert.Equal("Test", stringValueProperty.GetValue(loadedEntity));
        Assert.Equal(12, intValueProperty.GetValue(loadedEntity));
    }
    
    #endregion
}