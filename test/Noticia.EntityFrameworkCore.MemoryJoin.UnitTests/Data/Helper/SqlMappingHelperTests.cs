using System;
using System.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.Data.Helper;
using Xunit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Data.Helper;

enum TestEnum
{
    First = 1,
    Second = 2,
    Third = 3
}

public class SqlMappingHelperTests
{
    #region Methods

    [Fact]
    public void Should_GetDbType_When_DbTypeCanBeFound()
    {
        Assert.Equal(DbType.String, SqlMappingHelper.GetDbType(typeof(string)));
    }

    [Fact]
    public void Should_GetUnderlyingDbType_When_NullableTypePassed()
    {
        Assert.Equal(DbType.Guid, SqlMappingHelper.GetDbType(typeof(Guid?)));
    }

    [Fact]
    public void Should_GetEnumDbType_When_EnumValuePassed()
    {
        Assert.Equal(DbType.Int32, SqlMappingHelper.GetDbType(typeof(TestEnum)));
    }

    [Fact]
    public void Should_ThrowArgumentException_When_DbTypeCanNotBeFound()
    {
        Assert.Throws<ArgumentException>(() => SqlMappingHelper.GetDbType(typeof(SqlMappingHelperTests)));
    }
    
    #endregion
}