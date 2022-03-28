using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

public class MemoryDataSqlServiceBase : IMemoryDataSqlService
{
    /// <inheritdoc />
    public string GetSql()
    {
        var type = typeof(MemoryDataSqlServiceBase);
        
        var allowedMapping = type.
            GetProperties().
            Where(x => x.GetCustomAttribute<KeyAttribute>() == null).
            GroupBy(x => Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType).
            ToDictionary(x => x.Key, x => x.ToArray());
        
        
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void AppendNullRow()
    {
        throw new NotImplementedException();
    }
}