﻿using System;
using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Reflection;

public class MemoryEntityBuilderTests
{
    #region Fields

    private readonly MemoryEntityTypeBuilder memoryEntityTypeBuilder;

    #endregion

    #region Constructors

    public MemoryEntityBuilderTests()
    {
        this.memoryEntityTypeBuilder = new MemoryEntityTypeBuilder();
    }
    
    #endregion

    #region Methods

    [Fact]
    public void Should_InheritProperties_When_ModelGiven()
    {
        var type = this.memoryEntityTypeBuilder.Build<TestModel>();
        
        Assert.NotNull(type.GetProperty("StringValue"));
        Assert.NotNull(type.GetProperty("IntValue"));
    }
    
    [Fact]
    public void Should_AddIdProperty_When_ModelGiven()
    {
        var type = this.memoryEntityTypeBuilder.Build<TestModel>();
        
        Assert.NotNull(type.GetProperty("Id"));
    }
    
    [Fact]
    public void Should_SetAndGetIdProperty_When_Called()
    {
        var type = this.memoryEntityTypeBuilder.Build<TestModel>();
        var propertyInfo = type.GetProperty("Id") ?? throw new InvalidOperationException("Id property not found.");
        var instance = Activator.CreateInstance(type);

        propertyInfo.SetValue(instance, 42);
        
        Assert.Equal(42, propertyInfo.GetValue(instance));
    }
 
    [Fact]
    public void Should_ThrowInvalidOperationException_When_IdModelGiven()
    {
        Assert.Throws<InvalidOperationException>(() => this.memoryEntityTypeBuilder.Build<IdTestModel>());
    }
    
    #endregion
}