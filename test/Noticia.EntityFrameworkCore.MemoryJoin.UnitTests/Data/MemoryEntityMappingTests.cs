using System;
using System.Linq;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Data;

public class MemoryEntityMappingTests
{
    #region Fields

    private readonly MemoryEntityMapping<TestModel> memoryEntityMapping;

    #endregion

    #region Constructors

    public MemoryEntityMappingTests()
    {
        this.memoryEntityMapping = new MemoryEntityMapping<TestModel>();
    }

    #endregion

    #region Methods

    [Fact]
    public void Should_GetModelProperties_When_GivenValidModel()
    {
        var propertyNames = new[] { "StringValue", "IntValue" };

        Assert.Equal(propertyNames,
            this.memoryEntityMapping.ModelProperties.Select(modelProperty => modelProperty.Name));
    }

    [Fact]
    public void Should_GetMemoryEntityProperties_When_GivenValidModel()
    {
        var propertyNames = new[] { "Id", "StringValue", "IntValue" };

        Assert.Equal(propertyNames,
            this.memoryEntityMapping.MemoryEntityProperties.Select(memoryEntityProperty => memoryEntityProperty.Name));
    }

    [Fact]
    public void Should_MapToMemoryEntity_When_ModelGiven()
    {
        var model = new TestModel() { StringValue = "abcd", IntValue = 42 };
        var toFunc = (Func<TestModel, object>)this.memoryEntityMapping.ToExpression.Compile();

        var memoryEntity = toFunc(model);

        Assert.Equal(0,
            this.memoryEntityMapping.MemoryEntityProperties
                .Single(memoryEntityProperty => memoryEntityProperty.Name == "Id").GetValue(memoryEntity));
        Assert.Equal("abcd",
            this.memoryEntityMapping.MemoryEntityProperties
                .Single(memoryEntityProperty => memoryEntityProperty.Name == "StringValue").GetValue(memoryEntity));
        Assert.Equal(42,
            this.memoryEntityMapping.MemoryEntityProperties
                .Single(memoryEntityProperty => memoryEntityProperty.Name == "IntValue").GetValue(memoryEntity));
    }

    [Fact]
    public void Should_MapToModel_When_MemoryEntityGiven()
    {
        var memoryEntity = Activator.CreateInstance(MemoryEntityManager.GetMemoryEntityType<TestModel>()) ?? throw new NullReferenceException();

        this.memoryEntityMapping.MemoryEntityProperties
            .Single(memoryEntityProperty => memoryEntityProperty.Name == "Id").SetValue(memoryEntity, 0);
        this.memoryEntityMapping.MemoryEntityProperties
            .Single(memoryEntityProperty => memoryEntityProperty.Name == "StringValue").SetValue(memoryEntity, "abcd");
        this.memoryEntityMapping.MemoryEntityProperties
            .Single(memoryEntityProperty => memoryEntityProperty.Name == "IntValue").SetValue(memoryEntity, 42);

        var fromFunc = this.memoryEntityMapping.FromExpression.Compile();
        
        var model =  fromFunc.DynamicInvoke(memoryEntity);

        Assert.Equal("abcd",
            this.memoryEntityMapping.ModelProperties.Single(modelProperty => modelProperty.Name == "StringValue")
                .GetValue(model));
        Assert.Equal(42,
            this.memoryEntityMapping.ModelProperties.Single(modelProperty => modelProperty.Name == "IntValue")
                .GetValue(model));
    }

    #endregion
}